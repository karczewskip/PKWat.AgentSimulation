namespace PKWat.AgentSimulation.Core
{
    using PKWat.AgentSimulation.Core.Snapshots;

    public interface ISimulation
    {
        public bool Running { get; }
        public SimulationCrashResult Crash { get; }

        Task StartAsync();
        Task StopAsync();
    }

    internal class Simulation<T, ENVIRONMENT_STATE> : ISimulation where T : ISimulationEnvironment<ENVIRONMENT_STATE> where ENVIRONMENT_STATE : ISnapshotCreator
    {
        private readonly SimulationContext<T, ENVIRONMENT_STATE> _context;
        private readonly SimulationSnapshotStore _snapshotStore;
        private readonly IReadOnlyList<Func<SimulationContext<T, ENVIRONMENT_STATE>, Task>> _callbacks;
        private readonly IReadOnlyList<ISimulationEvent<T>> _events;
        private readonly IReadOnlyList<Func<SimulationContext<T, ENVIRONMENT_STATE>, SimulationCrashResult>> _crashConditions;

        public bool Running { get; private set; } = false;
        public SimulationCrashResult Crash { get; private set; } = SimulationCrashResult.NoCrash;

        public Simulation(
            SimulationContext<T, ENVIRONMENT_STATE> context,
            SimulationSnapshotStore simulationSnapshotStore,
            IReadOnlyList<Func<SimulationContext<T, ENVIRONMENT_STATE>, Task>> callbacks,
            IReadOnlyList<ISimulationEvent<T>> events,
            IReadOnlyList<Func<SimulationContext<T, ENVIRONMENT_STATE>, SimulationCrashResult>> crashConditions)
        {
            _context = context;
            _snapshotStore = simulationSnapshotStore;
            _callbacks = callbacks;
            _events = events;
            _crashConditions = crashConditions;
        }

        public async Task StartAsync()
        {
            Running = true;

            await Parallel.ForEachAsync(
                    _context.Agents,
                    new ParallelOptions() { MaxDegreeOfParallelism = 2 },
                    (x, c) => new ValueTask(Task.Run(() => x.Value.Initialize(_context))));

            _snapshotStore.CleanExistingSnapshots();

            while (Running)
            {
                foreach (var agentToRemove in _context.Agents.Values.Where(x => x.ShouldBeRemovedFromSimulation(_context)))
                {
                    _context.Agents.Remove(agentToRemove.Id);
                }

                _context.SimulationEnvironment.Update(_context.SimulationEnvironmentState);

                foreach (var simulationEvent in _events)
                {
                    if(await simulationEvent.ShouldBeExecuted(_context))
                    {
                        await simulationEvent.Execute(_context);
                    }
                }

                await Parallel.ForEachAsync(
                    _context.Agents,
                    new ParallelOptions() { MaxDegreeOfParallelism = 2 },
                    (x, c) => new ValueTask(Task.Run(() => x.Value.Prepare(_context))));

                await Parallel.ForEachAsync(
                    _context.Agents, 
                    new ParallelOptions() { MaxDegreeOfParallelism = 2 },
                    (x, c) => new ValueTask(Task.Run( () => x.Value.Act())));

                foreach (var callback in _callbacks)
                {
                    await callback(_context);
                }

                _context.Update();

                await _snapshotStore.SaveSnapshotAsync(
                    new SimulationSnapshot(new SimulationTimeSnapshot(_context.SimulationTime), 
                    new SimulationEnvironmentSnapshot(_context.SimulationEnvironmentState.CreateSnapshot()), 
                    _context.Agents.Select(x => new SimulationAgentSnapshot(x.Value.GetType().FullName, x.Key, x.Value.CreateSnapshot())).ToArray()),
                    default);

                foreach (var crashCondition in _crashConditions)
                {
                    var crashResult = crashCondition(_context);

                    if (crashResult.IsCrash)
                    {
                        Crash = crashResult;
                        Running = false;
                        break;
                    }
                }

                await Task.Delay(_context.WaitingTimeBetweenSteps);
            }
        }

        public async Task StopAsync()
        {
            Running = false;
        }
    }
}
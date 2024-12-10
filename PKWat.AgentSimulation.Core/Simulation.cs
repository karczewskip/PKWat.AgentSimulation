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

    internal class Simulation<T, ENVIRONMENT_STATE> : ISimulation where T : ISimulationEnvironment<ENVIRONMENT_STATE>
    {
        private readonly SimulationContext<T> _context;
        private readonly SimulationSnapshotStore _snapshotStore;
        private readonly IReadOnlyList<Func<SimulationContext<T>, Task>> _environmentUpdates;
        private readonly IReadOnlyList<Func<SimulationContext<T>, Task>> _callbacks;
        private readonly IReadOnlyList<ISimulationEvent<T>> _events;

        public bool Running { get; private set; } = false;
        public SimulationCrashResult Crash { get; private set; } = SimulationCrashResult.NoCrash;

        public Simulation(
            SimulationContext<T> context,
            SimulationSnapshotStore simulationSnapshotStore,
            IReadOnlyList<Func<SimulationContext<T>, Task>> environmentUpdates,
            IReadOnlyList<Func<SimulationContext<T>, Task>> callbacks,
            IReadOnlyList<ISimulationEvent<T>> events)
        {
            _context = context;
            _snapshotStore = simulationSnapshotStore;
            _environmentUpdates = environmentUpdates;
            _callbacks = callbacks;
            _events = events;
        }

        public async Task StartAsync()
        {
            Running = true;

            await Parallel.ForEachAsync(
                    _context.Agents,
                    new ParallelOptions() { MaxDegreeOfParallelism = 2 },
                    (x, c) => new ValueTask(Task.Run(() => x.Value.Initialize(_context.SimulationEnvironment))));

            _snapshotStore.CleanExistingSnapshots();

            while (Running)
            {
                foreach (var agentToRemove in _context.Agents.Values.Where(x => x.ShouldBeRemovedFromSimulation(_context.SimulationTime)))
                {
                    _context.Agents.Remove(agentToRemove.Id);
                }

                foreach (var environmentUpdate in _environmentUpdates)
                {
                    await environmentUpdate(_context);
                }

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
                    (x, c) => new ValueTask(Task.Run(() => x.Value.Prepare(_context.SimulationEnvironment, _context.SimulationTime))));

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
                    new SimulationEnvironmentSnapshot(_context.SimulationEnvironment.CreateSnapshot()), 
                    _context.Agents.Select(x => new SimulationAgentSnapshot(x.Value.GetType().FullName, x.Key, x.Value.CreateSnapshot())).ToArray()),
                    default);

                var crashResult = _context.SimulationEnvironment.CheckCrashConditions();

                if (crashResult.IsCrash)
                {
                    Crash = crashResult;
                    Running = false;
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
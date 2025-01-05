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
        private readonly ISimulationSnapshotStore _snapshotStore;
        private readonly IReadOnlyList<Func<SimulationContext<T>, Task>> _environmentUpdates;
        private readonly Func<SimulationContext<T>, Task> _environmentInitialization;
        private readonly IReadOnlyList<Func<SimulationContext<T>, Task>> _callbacks;
        private readonly IReadOnlyList<ISimulationEvent<T>> _events;

        private RunningSimulationState _runningState = RunningSimulationState.CreateNotRunningState();

        public bool Running => _runningState.IsRunning;
        public SimulationCrashResult Crash => _runningState.CrashResult;

        public Simulation(
            SimulationContext<T> context,
            ISimulationSnapshotStore simulationSnapshotStore,
            IReadOnlyList<Func<SimulationContext<T>, Task>> environmentUpdates,
            Func<SimulationContext<T>, Task> environmentInitialization,
            IReadOnlyList<Func<SimulationContext<T>, Task>> callbacks,
            IReadOnlyList<ISimulationEvent<T>> events)
        {
            _context = context;
            _snapshotStore = simulationSnapshotStore;
            _environmentUpdates = environmentUpdates;
            _environmentInitialization = environmentInitialization;
            _callbacks = callbacks;
            _events = events;
        }

        public async Task StartAsync()
        {
            _runningState = RunningSimulationState.CreateRunningState();

            await _environmentInitialization(_context);

            await Parallel.ForEachAsync(
                    _context.Agents,
                    new ParallelOptions() { MaxDegreeOfParallelism = 10 },
                    (x, c) => new ValueTask(Task.Run(() => x.Value.Initialize(_context.SimulationEnvironment))));

            _snapshotStore.CleanExistingSnapshots();

            while (Running)
            {
                await _snapshotStore.SaveSnapshotAsync(
                    new SimulationSnapshot(new SimulationTimeSnapshot(_context.SimulationTime),
                    new SimulationEnvironmentSnapshot(_context.SimulationEnvironment.CreateSnapshot()),
                    _context.Agents.Select(x => new SimulationAgentSnapshot(x.Value.GetType().FullName, x.Key, x.Value.CreateSnapshot())).ToArray()),
                    _runningState.CancellationToken);

                _context.StartNewCycle();

                foreach (var agentToRemove in _context.Agents.Values.Where(x => x.ShouldBeRemovedFromSimulation(_context.SimulationTime)))
                {
                    _context.Agents.Remove(agentToRemove.Id);
                }

                using (_context.PerformanceInfo.AddStep("Environment update"))
                {
                    foreach (var environmentUpdate in _environmentUpdates)
                    {
                        await environmentUpdate(_context);
                    }
                }

                foreach (var simulationEvent in _events)
                {
                    if(await simulationEvent.ShouldBeExecuted(_context))
                    {
                        await simulationEvent.Execute(_context);
                    }
                }

                using (_context.PerformanceInfo.AddStep("Agents update"))
                {
                    await Parallel.ForEachAsync(
                        _context.Agents,
                        new ParallelOptions() { MaxDegreeOfParallelism = 4 },
                        (x, c) => new ValueTask(Task.Run(() => x.Value.Act(_context.SimulationEnvironment, _context.SimulationTime))));
                }

                foreach (var callback in _callbacks)
                {
                    await callback(_context);
                }

                var crashResult = _context.SimulationEnvironment.CheckCrashConditions();

                if (crashResult.IsCrash)
                {
                    _runningState.Crash(crashResult);
                }

                await Task.Delay(_context.WaitingTimeBetweenSteps);
            }
        }

        public async Task StopAsync()
        {
            _runningState.Stop();
        }

        private class RunningSimulationState
        {
            private CancellationTokenSource? _cancellationTokenSource;

            public bool IsRunning { get; private set; }
            public CancellationToken CancellationToken { get; private set; }
            public SimulationCrashResult CrashResult { get; private set; } = SimulationCrashResult.NoCrash;

            private RunningSimulationState(
                CancellationTokenSource? cancellationTokenSource, 
                bool isRunning, 
                CancellationToken cancellationToken)
            {
                _cancellationTokenSource = cancellationTokenSource;

                IsRunning = isRunning;
                CancellationToken = cancellationToken;
            }

            public static RunningSimulationState CreateRunningState()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                return new RunningSimulationState(cancellationTokenSource, true, cancellationToken);
            }

            public static RunningSimulationState CreateNotRunningState()
            {
                return new RunningSimulationState(null, false, CancellationToken.None);
            }

            public void Stop()
            {
                IsRunning = false;
                _cancellationTokenSource?.Cancel();
            }

            public void Crash(SimulationCrashResult crash)
            {
                CrashResult = crash;
                Stop();
            }
        }
    }
}
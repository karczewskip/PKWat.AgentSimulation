namespace PKWat.AgentSimulation.Core
{
    using PKWat.AgentSimulation.Core.Crash;
    using PKWat.AgentSimulation.Core.Environment;
    using PKWat.AgentSimulation.Core.Snapshots;
    using PKWat.AgentSimulation.Core.Stage;

    public interface ISimulation
    {
        public bool Running { get; }
        public SimulationCrashResult Crash { get; }

        Task StartAsync();
        Task StopAsync();
    }

    internal class Simulation<T> : ISimulation where T : ISimulationEnvironment
    {
        private readonly SimulationContext<T> _context;
        private readonly ISimulationSnapshotStore _snapshotStore;
        private readonly IReadOnlyList<Func<SimulationContext<T>, Task>> _callbacks;
        private readonly ISimulationStage<T>[] _initializationStages;
        private readonly ISimulationStage<T>[] _stages;
        private readonly bool _runAgentsInParallel;

        private RunningSimulationState _runningState = RunningSimulationState.CreateNotRunningState();

        public bool Running => _runningState.IsRunning;
        public SimulationCrashResult Crash => _runningState.CrashResult;

        public Simulation(
            SimulationContext<T> context,
            ISimulationSnapshotStore simulationSnapshotStore,
            IReadOnlyList<Func<SimulationContext<T>, Task>> callbacks,
            ISimulationStage<T>[] initializationStages,
            ISimulationStage<T>[] stages,
            bool runAgentsInParallel)
        {
            _context = context;
            _snapshotStore = simulationSnapshotStore;
            _callbacks = callbacks;
            _initializationStages = initializationStages;
            _stages = stages;
            _runAgentsInParallel = runAgentsInParallel;
        }

        public async Task StartAsync()
        {
            _runningState = RunningSimulationState.CreateRunningState();

            foreach (var stage in _initializationStages)
            {
                await stage.Execute(_context);
            }

            await Parallel.ForEachAsync(
                    _context.Agents,
                    new ParallelOptions() { MaxDegreeOfParallelism = 10 },
                    (x, c) => new ValueTask(Task.Run(() => x.Value.Initialize(_context.SimulationEnvironment))));

            _snapshotStore.CleanExistingSnapshots();

            _context.StartCycleZero();

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

                foreach (var stage in _stages)
                {
                    await stage.Execute(_context);
                }

                //using (_context.PerformanceInfo.AddStep("Agents update"))
                if (_runAgentsInParallel)
                {
                    var agentsCount = _context.Agents.Count;

                    if(agentsCount > 0)
                    {
                        var numberOfThreads = 8;
                        var chunkSize = 1 + (agentsCount - 1) / numberOfThreads;
                        var chunkedAgents = _context.Agents.Values.Chunk(chunkSize).ToArray();

                        await Parallel.ForEachAsync(
                            chunkedAgents,
                            new ParallelOptions() { MaxDegreeOfParallelism = numberOfThreads },
                            (chunk, c) => new ValueTask(Task.Run(() =>
                            {
                                using var step = _context.PerformanceInfo.AddStep("Chunk");
                                foreach (var agent in chunk)
                                {
                                    agent.Act(_context.SimulationEnvironment, _context.SimulationTime);
                                }
                            })));
                    }
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
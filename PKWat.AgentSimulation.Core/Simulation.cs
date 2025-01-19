namespace PKWat.AgentSimulation.Core
{
    using PKWat.AgentSimulation.Core.Crash;
    using PKWat.AgentSimulation.Core.Snapshots;
    using PKWat.AgentSimulation.Core.Stage;

    public interface ISimulation
    {
        public bool Running { get; }
        public SimulationCrashResult Crash { get; }

        Task StartAsync();
        Task StopAsync();
    }

    internal class Simulation : ISimulation
    {
        private readonly SimulationContext _context;
        private readonly ISimulationSnapshotStore _snapshotStore;
        private readonly IReadOnlyList<Func<SimulationContext, Task>> _callbacks;
        private readonly ISimulationStage[] _initializationStages;
        private readonly ISimulationStage[] _stages;

        public bool Running => _context.IsRunning;
        public SimulationCrashResult Crash => _context.CrashResult;

        public Simulation(
            SimulationContext context,
            ISimulationSnapshotStore simulationSnapshotStore,
            IReadOnlyList<Func<SimulationContext, Task>> callbacks,
            ISimulationStage[] initializationStages,
            ISimulationStage[] stages)
        {
            _context = context;
            _snapshotStore = simulationSnapshotStore;
            _callbacks = callbacks;
            _initializationStages = initializationStages;
            _stages = stages;
        }

        public async Task StartAsync()
        {
            _context.StartSimulation();

            foreach (var stage in _initializationStages)
            {
                await stage.Execute(_context);
            }

            _snapshotStore.CleanExistingSnapshots();

            _context.StartCycleZero();

            while (Running)
            {
                await _snapshotStore.SaveSnapshotAsync(_context);

                _context.StartNewCycle();

                foreach (var stage in _stages)
                {
                    await stage.Execute(_context);
                }

                foreach (var callback in _callbacks)
                {
                    await callback(_context);
                }

                var crashResult = _context.SimulationEnvironment.CheckCrashConditions();

                if (crashResult.IsCrash)
                {
                    _context.Crash(crashResult);
                }

                await _context.OnCycleFinishAsync();
            }
        }

        public async Task StopAsync()
        {
            _context.StopSimulation();
        }
    }
}
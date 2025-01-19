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

    internal class Simulation(
            SimulationContext context,
            ISimulationSnapshotStore snapshotStore,
            IReadOnlyList<Func<SimulationContext, Task>> callbacks,
            ISimulationStage[] initializationStages,
            ISimulationStage[] stages,
            IReadOnlyList<Func<ISimulationContext, SimulationCrashResult>> crashConditions) : ISimulation
    {
        public bool Running => context.IsRunning;
        public SimulationCrashResult Crash => context.CrashResult;

        public async Task StartAsync()
        {
            context.StartSimulation();

            foreach (var stage in initializationStages)
                await stage.Execute(context);

            snapshotStore.CleanExistingSnapshots();
            context.StartCycleZero();

            while (Running)
            {
                await snapshotStore.SaveSnapshotAsync(context);

                context.StartNewCycle();

                foreach (var stage in stages)
                    await stage.Execute(context);

                foreach (var callback in callbacks)
                    await callback(context);

                foreach (var crashCondition in crashConditions)
                {
                    var crashResult = crashCondition(context);
                    if (crashResult.IsCrash)
                    {
                        context.Crash(crashResult);
                    }
                }

                await context.OnCycleFinishAsync();
            }
        }

        public async Task StopAsync()
        {
            context.StopSimulation();
        }
    }
}
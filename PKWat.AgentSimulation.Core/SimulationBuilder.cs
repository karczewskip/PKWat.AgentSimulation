namespace PKWat.AgentSimulation.Core;

using PKWat.AgentSimulation.Core.Snapshots;

public interface ISimulationBuilder
{
    ISimulationBuilderContext<T, ENVIRONMENT_STATE> CreateNewSimulation<T, ENVIRONMENT_STATE>(ENVIRONMENT_STATE simulationState) where T : ISimulationEnvironment<ENVIRONMENT_STATE> where ENVIRONMENT_STATE : ISnapshotCreator;
}

internal class SimulationBuilder : ISimulationBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public SimulationBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISimulationBuilderContext<T, ENVIRONMENT_STATE> CreateNewSimulation<T, ENVIRONMENT_STATE>(ENVIRONMENT_STATE simulationState) where T : ISimulationEnvironment<ENVIRONMENT_STATE> where ENVIRONMENT_STATE : ISnapshotCreator
    {
        var builderContext = new SimulationBuilderContext<T, ENVIRONMENT_STATE>(_serviceProvider);
        builderContext.LoadState(simulationState);

        return builderContext;
    }
}

using PKWat.AgentSimulation.Core.Environment;

namespace PKWat.AgentSimulation.Core.Builder;

public interface ISimulationBuilder
{
    ISimulationBuilderContext<T, ENVIRONMENT_STATE> CreateNewSimulation<T, ENVIRONMENT_STATE>(ENVIRONMENT_STATE simulationState) where T : ISimulationEnvironment<ENVIRONMENT_STATE>;
}

internal class SimulationBuilder : ISimulationBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public SimulationBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISimulationBuilderContext<T, ENVIRONMENT_STATE> CreateNewSimulation<T, ENVIRONMENT_STATE>(ENVIRONMENT_STATE simulationState) where T : ISimulationEnvironment<ENVIRONMENT_STATE>
    {
        var builderContext = new SimulationBuilderContext<T, ENVIRONMENT_STATE>(_serviceProvider);
        builderContext.LoadState(simulationState);

        return builderContext;
    }
}

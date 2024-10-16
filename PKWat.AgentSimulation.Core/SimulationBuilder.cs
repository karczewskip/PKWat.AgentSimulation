namespace PKWat.AgentSimulation.Core;

using PKWat.AgentSimulation.Core.Snapshots;

public interface ISimulationBuilder
{
    ISimulationBuilderContext<T, ENVIRONMENT_STATE> CreateNewSimulation<T, ENVIRONMENT_STATE>(T simulationEnvironment) where T : ISimulationEnvironment<ENVIRONMENT_STATE>;
}

internal class SimulationBuilder : ISimulationBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public SimulationBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISimulationBuilderContext<T, ENVIRONMENT_STATE> CreateNewSimulation<T, ENVIRONMENT_STATE>(T simulationEnvironment) where T : ISimulationEnvironment<ENVIRONMENT_STATE>
    {
        return new SimulationBuilderContext<T, ENVIRONMENT_STATE>(simulationEnvironment, _serviceProvider);
    }
}

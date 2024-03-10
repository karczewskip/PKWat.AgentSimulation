namespace PKWat.AgentSimulation.Core;

public interface ISimulationBuilder
{
    ISimulationBuilderContext<T> CreateNewSimulation<T>(T simulationEnvironment);
}

internal class SimulationBuilder : ISimulationBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public SimulationBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISimulationBuilderContext<T> CreateNewSimulation<T>(T simulationEnvironment)
    {
        return new SimulationBuilderContext<T>(simulationEnvironment, _serviceProvider);
    }
}

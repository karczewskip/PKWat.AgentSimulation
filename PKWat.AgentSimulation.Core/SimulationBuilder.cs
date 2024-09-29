namespace PKWat.AgentSimulation.Core;

using PKWat.AgentSimulation.Core.Snapshots;

public interface ISimulationBuilder
{
    ISimulationBuilderContext<T> CreateNewSimulation<T>(T simulationEnvironment) where T : ISimulationEnvironment;
}

internal class SimulationBuilder : ISimulationBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public SimulationBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISimulationBuilderContext<T> CreateNewSimulation<T>(T simulationEnvironment) where T : ISimulationEnvironment
    {
        return new SimulationBuilderContext<T>(simulationEnvironment, _serviceProvider);
    }
}

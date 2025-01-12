using PKWat.AgentSimulation.Core.Environment;

namespace PKWat.AgentSimulation.Core.Builder;

public interface ISimulationBuilder
{
    ISimulationBuilderContext CreateNewSimulation<T>() where T : ISimulationEnvironment;
}

internal class SimulationBuilder : ISimulationBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public SimulationBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISimulationBuilderContext CreateNewSimulation<T>() where T : ISimulationEnvironment
    {
        var builderContext = new SimulationBuilderContext(_serviceProvider, typeof(T));

        return builderContext;
    }
}

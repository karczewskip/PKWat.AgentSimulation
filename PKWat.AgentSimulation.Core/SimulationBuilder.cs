namespace PKWat.AgentSimulation.Core;

public interface ISimulationBuilder
{
    ISimulationBuilderContext<T> CreateNewSimulation<T>(T simulationEnvironment);
}

internal class SimulationBuilder : ISimulationBuilder
{
    public ISimulationBuilderContext<T> CreateNewSimulation<T>(T simulationEnvironment)
    {
        return new SimulationBuilderContext<T>(simulationEnvironment);
    }
}

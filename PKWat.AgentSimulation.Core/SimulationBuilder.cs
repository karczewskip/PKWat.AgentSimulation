namespace PKWat.AgentSimulation.Core;

public interface ISimulationBuilder
{
    ISimulationBuilderContext CreateNewSimulation();
}

internal class SimulationBuilder : ISimulationBuilder
{
    public ISimulationBuilderContext CreateNewSimulation()
    {
        return new SimulationBuilderContext();
    }
}

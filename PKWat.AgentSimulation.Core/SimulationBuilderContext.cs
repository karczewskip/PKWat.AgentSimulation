namespace PKWat.AgentSimulation.Core;

public interface ISimulationBuilderContext
{
    ISimulationBuilderContext AddAgent(IAgent agent);
    ISimulation Build();
}

internal class SimulationBuilderContext : ISimulationBuilderContext
{
    private List<IAgent> _agents = new List<IAgent>();

    public ISimulationBuilderContext AddAgent(IAgent agent)
    {
        _agents.Add(agent);

        return this;
    }

    public ISimulation Build()
    {
        return new Simulation(new SimulationContext(_agents));
    }
}

namespace PKWat.AgentSimulation.Core;

public interface ISimulationBuilderContext
{
    ISimulationBuilderContext AddAgent(IAgent agent);
    ISimulationBuilderContext AddAgents(IEnumerable<IAgent> agents);
    ISimulationBuilderContext AddCallback(Func<Task> callback);
    ISimulationBuilderContext SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps);

    ISimulation Build();
}

internal class SimulationBuilderContext : ISimulationBuilderContext
{
    private List<IAgent> _agents = new List<IAgent>();
    private List<Func<Task>> _callbacks = new List<Func<Task>>();
    private TimeSpan _waitingTimeBetweenSteps = TimeSpan.Zero;

    public ISimulationBuilderContext AddAgent(IAgent agent)
    {
        _agents.Add(agent);

        return this;
    }

    public ISimulationBuilderContext AddAgents(IEnumerable<IAgent> agents)
    {
        _agents.AddRange(agents);

        return this;
    }

    public ISimulationBuilderContext AddCallback(Func<Task> callback)
    {
        _callbacks.Add(callback);

        return this;
    }

    public ISimulationBuilderContext SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps)
    {
        _waitingTimeBetweenSteps = waitingTimeBetweenSteps;

        return this;
    }

    public ISimulation Build()
    {
        return new Simulation(new SimulationContext(_agents, _callbacks, _waitingTimeBetweenSteps));
    }
}

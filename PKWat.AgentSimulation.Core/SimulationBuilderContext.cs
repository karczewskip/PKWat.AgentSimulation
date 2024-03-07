namespace PKWat.AgentSimulation.Core;

public interface ISimulationBuilderContext<T>
{
    ISimulationBuilderContext<T> AddAgent(IAgent<T> agent);
    ISimulationBuilderContext<T> AddAgents(IEnumerable<IAgent<T>> agents);
    ISimulationBuilderContext<T> AddCallback(Func<ISimulationContext<T>, Task> callback);
    ISimulationBuilderContext<T> SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps);

    ISimulation Build();
}

internal class SimulationBuilderContext<T> : ISimulationBuilderContext<T>
{
    private readonly T _simulationEnvironment;

    private List<IAgent<T>> _agents = new();
    private List<Func<ISimulationContext<T>, Task>> _callbacks = new();
    private TimeSpan _waitingTimeBetweenSteps = TimeSpan.Zero;

    public SimulationBuilderContext(T simulationEnvironment)
    {
        _simulationEnvironment = simulationEnvironment;
    }

    public ISimulationBuilderContext<T> AddAgent(IAgent<T> agent)
    {
        _agents.Add(agent);

        return this;
    }

    public ISimulationBuilderContext<T> AddAgents(IEnumerable<IAgent<T>> agents)
    {
        _agents.AddRange(agents);

        return this;
    }

    public ISimulationBuilderContext<T> AddCallback(Func<ISimulationContext<T>, Task> callback)
    {
        _callbacks.Add(callback);

        return this;
    }

    public ISimulationBuilderContext<T> SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps)
    {
        _waitingTimeBetweenSteps = waitingTimeBetweenSteps;

        return this;
    }

    public ISimulation Build()
    {
        return new Simulation<T>(new SimulationContext<T>(_simulationEnvironment ,_agents, _waitingTimeBetweenSteps), _callbacks);
    }
}

namespace PKWat.AgentSimulation.Core;

public interface ISimulationAgent<ENVIRONMENT> : IRecognizableAgent
{
    void Initialize(ISimulationContext<ENVIRONMENT> simulationContext);
    void Prepare(ISimulationContext<ENVIRONMENT> simulationContext);
    void Act();
}

public record AgentId
{
    public static AgentId Empty { get; } = new AgentId(Guid.Empty);

    public Guid Id { get; }

    private AgentId(Guid id)
    {
        Id = id;
    }

    public static AgentId GenerateNew() => new AgentId(Guid.NewGuid());
}

public interface IRecognizableAgent : IEquatable<IRecognizableAgent>
{
    AgentId Id { get; }
}

public abstract class SimulationAgent<ENVIRONMENT, STATE> : ISimulationAgent<ENVIRONMENT>
{
    private STATE _nextState;

    public STATE State { get; private set; }

    public AgentId Id { get; } = AgentId.GenerateNew();

    public void Initialize(ISimulationContext<ENVIRONMENT> simulationContext)
    {
        State = GetInitialState(simulationContext.SimulationEnvironment);
    }

    public void Prepare(ISimulationContext<ENVIRONMENT> simulationContext)
    {
        _nextState = GetNextState(simulationContext.SimulationEnvironment, simulationContext.SimulationTime);
    }

    public void Act()
    {
        State = _nextState;
    }

    protected abstract STATE GetInitialState(ENVIRONMENT environment);
    protected abstract STATE GetNextState(ENVIRONMENT environment, SimulationTime simulationTime);

    public bool Equals(IRecognizableAgent? other)
    {
        return other is IRecognizableAgent agent && agent.Id == Id;
    }
}

namespace PKWat.AgentSimulation.Core;

using PKWat.AgentSimulation.Core.Snapshots;

public interface ISimulationAgent
{

}

public interface ISimulationAgent<ENVIRONMENT, ENVIRONMENT_STATE> : ISimulationAgent, IRecognizableAgent, ISnapshotCreator where ENVIRONMENT : ISimulationEnvironment<ENVIRONMENT_STATE>
{
    void Initialize(ISimulationContext<ENVIRONMENT, ENVIRONMENT_STATE> simulationContext);
    void Prepare(ISimulationContext<ENVIRONMENT, ENVIRONMENT_STATE> simulationContext);
    void Act();
    bool ShouldBeRemovedFromSimulation(ISimulationContext<ENVIRONMENT, ENVIRONMENT_STATE> simulationContext);
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

    public override string ToString() => Id.ToString();
}

public interface IRecognizableAgent : IEquatable<IRecognizableAgent>
{
    AgentId Id { get; }
}

public abstract class SimulationAgent<ENVIRONMENT, ENVIRONMENT_STATE, STATE> : ISimulationAgent<ENVIRONMENT, ENVIRONMENT_STATE> where ENVIRONMENT : ISimulationEnvironment<ENVIRONMENT_STATE>
{
    private STATE _nextState;

    public STATE State { get; private set; }

    public AgentId Id { get; } = AgentId.GenerateNew();

    public void Initialize(ISimulationContext<ENVIRONMENT, ENVIRONMENT_STATE> simulationContext)
    {
        State = GetInitialState(simulationContext.SimulationEnvironment);
    }

    public void Prepare(ISimulationContext<ENVIRONMENT, ENVIRONMENT_STATE> simulationContext)
    {
        _nextState = GetNextState(simulationContext.SimulationEnvironment, simulationContext.SimulationTime);
    }

    public void Act()
    {
        State = _nextState;
    }

    public virtual bool ShouldBeRemovedFromSimulation(ISimulationContext<ENVIRONMENT, ENVIRONMENT_STATE> simulationContext)
    {
        return false;
    }

    protected abstract STATE GetInitialState(ENVIRONMENT environment);
    protected abstract STATE GetNextState(ENVIRONMENT environment, SimulationTime simulationTime);

    protected void SetState(STATE nextState)
    {
        State = nextState;
    }

    public bool Equals(IRecognizableAgent? other)
    {
        return other is IRecognizableAgent agent && agent.Id == Id;
    }

    public virtual object CreateSnapshot()
    {
        return State;
    }
}

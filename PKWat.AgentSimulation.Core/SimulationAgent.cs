namespace PKWat.AgentSimulation.Core;

using PKWat.AgentSimulation.Core.Snapshots;
using PKWat.AgentSimulation.Core.Time;

public interface ISimulationAgent
{

}

public interface ISimulationAgent<ENVIRONMENT> : ISimulationAgent, IRecognizableAgent, ISnapshotCreator where ENVIRONMENT : ISimulationEnvironment
{
    void Initialize(ENVIRONMENT environment);
    void Act(ENVIRONMENT environment, IReadOnlySimulationTime simulationTime);
    bool ShouldBeRemovedFromSimulation(IReadOnlySimulationTime simulationTime);
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

public abstract class SimulationAgent<ENVIRONMENT, STATE> : ISimulationAgent<ENVIRONMENT> where ENVIRONMENT : ISimulationEnvironment
{
    public STATE State { get; private set; }

    public AgentId Id { get; } = AgentId.GenerateNew();

    public void Initialize(ENVIRONMENT environment)
    {
        State = GetInitialState(environment);
    }

    public void Act(ENVIRONMENT environment, IReadOnlySimulationTime simulationTime)
    {
        State = GetNextState(environment, simulationTime);
    }

    public virtual bool ShouldBeRemovedFromSimulation(IReadOnlySimulationTime simulationTime)
    {
        return false;
    }

    protected abstract STATE GetInitialState(ENVIRONMENT environment);
    protected abstract STATE GetNextState(ENVIRONMENT environment, IReadOnlySimulationTime simulationTime);

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

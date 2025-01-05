namespace PKWat.AgentSimulation.Core.Agent;

using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.Time;

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

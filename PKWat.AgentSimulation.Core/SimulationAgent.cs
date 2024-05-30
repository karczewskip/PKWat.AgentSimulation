namespace PKWat.AgentSimulation.Core;

public interface IStateContainingAgent<ENVIRONMENT, STATE> : ISimulationAgent<ENVIRONMENT>
{
    void Initialize(STATE initialState);
}

public interface ISimulationAgent<ENVIRONMENT> : IRecognizableAgent
{
    void Prepare(IDictionary<Type, IPercept> percepts);
    void Act();

}

public interface IRecognizableAgent : IEquatable<IRecognizableAgent>
{
    Guid Id { get; }
}

public abstract class SimulationAgent<ENVIRONMENT, STATE> : IStateContainingAgent<ENVIRONMENT, STATE>
{
    private STATE _nextState;

    public STATE State { get; private set; }

    public Guid Id { get; } = Guid.NewGuid();

    public void Initialize(STATE initialState)
    {
        State = initialState;
    }

    public void Prepare(IDictionary<Type, IPercept> percepts)
    {
        _nextState = GetNextState(percepts);
    }

    public void Act()
    {
        State = _nextState;
    }

    protected abstract STATE GetNextState(IDictionary<Type, IPercept> percepts);

    public bool Equals(IRecognizableAgent? other)
    {
        return other is IRecognizableAgent agent && agent.Id == Id;
    }
}

namespace PKWat.AgentSimulation.Core;

public interface ISimulationAgent<ENVIRONMENT> : IRecognizableAgent
{
    void Initialize(ISimulationContext<ENVIRONMENT> simulationContext);
    void Prepare(ISimulationContext<ENVIRONMENT> simulationContext);
    void Act();
}

public interface IRecognizableAgent : IEquatable<IRecognizableAgent>
{
    Guid Id { get; }
}

public abstract class SimulationAgent<ENVIRONMENT, STATE> : ISimulationAgent<ENVIRONMENT>
{
    private STATE _nextState;

    public STATE State { get; private set; }

    public Guid Id { get; } = Guid.NewGuid();

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

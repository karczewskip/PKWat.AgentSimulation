namespace PKWat.AgentSimulation.Core;

public interface ISimulationAgent<ENVIRONMENT>
{
    void Initialize(ISimulationContext<ENVIRONMENT> simulationContext);
    void Decide(ISimulationContext<ENVIRONMENT> simulationContext);
    void Act();

}

public abstract class SimulationAgent<ENVIRONMENT, STATE> : ISimulationAgent<ENVIRONMENT>
{
    private STATE _nextState;

    public STATE State { get; private set; }


    public void Initialize(ISimulationContext<ENVIRONMENT> simulationContext)
    {
        State = GetInitialState(simulationContext);
    }

    public void Decide(ISimulationContext<ENVIRONMENT> simulationContext)
    {
        _nextState = GetNextState(simulationContext);
    }

    public void Act()
    {
        State = _nextState;
    }

    protected abstract STATE GetInitialState(ISimulationContext<ENVIRONMENT> simulationContext);
    protected abstract STATE GetNextState(ISimulationContext<ENVIRONMENT> simulationContext);
}

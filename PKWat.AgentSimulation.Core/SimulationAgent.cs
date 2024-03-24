namespace PKWat.AgentSimulation.Core;

public interface ISimulationAgent<T>
{
    void Initialize(ISimulationContext<T> simulationContext);
    void Decide(ISimulationContext<T> simulationContext);
    void Act();

}

public abstract class SimulationAgent<T, U> : ISimulationAgent<T>
{
    private U _nextState;

    public U State { get; private set; }


    public void Initialize(ISimulationContext<T> simulationContext)
    {
        State = GetInitialState(simulationContext);
    }

    public void Decide(ISimulationContext<T> simulationContext)
    {
        _nextState = GetNextState(simulationContext);
    }

    public void Act()
    {
        State = _nextState;
    }

    protected abstract U GetInitialState(ISimulationContext<T> simulationContext);
    protected abstract U GetNextState(ISimulationContext<T> simulationContext);
}

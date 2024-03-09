namespace PKWat.AgentSimulation.Core;

public interface IAgent<T>
{
    void Initialize(ISimulationContext<T> simulationContext);
    void Decide(ISimulationContext<T> simulationContext);
    void Act(ISimulationContext<T> simulationContext);
}

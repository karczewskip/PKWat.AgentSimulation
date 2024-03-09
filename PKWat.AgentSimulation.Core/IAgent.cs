namespace PKWat.AgentSimulation.Core;

public interface IAgent<T>
{
    void Initialize(T simulationEnvironment);
    void Decide(T simulationEnvironment);
    void Act(T simulationEnvironment);
}

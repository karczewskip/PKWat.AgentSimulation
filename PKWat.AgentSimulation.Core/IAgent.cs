namespace PKWat.AgentSimulation.Core;

public interface IAgent<T>
{
    void Decide(T simulationEnvironment);
    void Act(T simulationEnvironment);
}

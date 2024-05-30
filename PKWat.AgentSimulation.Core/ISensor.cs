namespace PKWat.AgentSimulation.Core;

public interface ISensor<ENVIRONMENT>
{
    IPercept Perceive(ENVIRONMENT environment, ISimulationAgent<ENVIRONMENT> agent);
}

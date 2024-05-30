namespace PKWat.AgentSimulation.Core;

public record AgentWithSensors<ENVIRONMENT>(ISimulationAgent<ENVIRONMENT> Agent, ISensor<ENVIRONMENT>[] Sensors)
{
    public void PrepareAgent(ENVIRONMENT environment) 
        => Agent.Prepare(GeneratePercepts(environment));

    private IDictionary<Type, IPercept> GeneratePercepts(ENVIRONMENT environment) 
        => Sensors.Select(x => x.Perceive(environment, Agent)).ToDictionary(x => x.GetType());
}

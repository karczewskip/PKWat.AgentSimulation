namespace PKWat.AgentSimulation.Core.Agent;
public abstract class SimpleSimulationAgent : ISimulationAgent
{
    public AgentId Id { get; } = AgentId.GenerateNew();

    public object CreateSnapshot()
    {
        return new { Id };
    }

    public bool Equals(IRecognizableAgent? other)
    {
        return other is IRecognizableAgent agent && agent.Id == Id;
    }
}

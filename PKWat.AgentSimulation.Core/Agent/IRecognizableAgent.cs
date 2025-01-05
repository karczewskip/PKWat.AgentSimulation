namespace PKWat.AgentSimulation.Core.Agent;

public interface IRecognizableAgent : IEquatable<IRecognizableAgent>
{
    AgentId Id { get; }
}

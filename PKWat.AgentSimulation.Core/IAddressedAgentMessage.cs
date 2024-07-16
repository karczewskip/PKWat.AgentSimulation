namespace PKWat.AgentSimulation.Core;

public interface IAddressedAgentMessage
{
    IRecognizableAgent Receiver { get; }
    IAgentMessage Message { get; }
}

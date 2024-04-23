namespace PKWat.AgentSimulation.Core;

public class AddressedAgentMessage : IAddressedAgentMessage
{
    public IRecognizableAgent Receiver { get; }

    public IAgentMessage Message { get; }

    public AddressedAgentMessage(IRecognizableAgent receiver, IAgentMessage message)
    {
        Receiver = receiver;
        Message = message;
    }
}

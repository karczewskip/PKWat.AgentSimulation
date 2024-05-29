namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Messages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

public record AskForLand(Airplane Sender) : IAgentMessage;

public record AllowLand() : IAgentMessage;

namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Messages;
using System.Linq;

public class Coordinator : SimulationAgent<AirportEnvironment, CoordinatorState>
{
    protected override CoordinatorState GetNextState(ISimulationContext<AirportEnvironment> simulationContext)
    {
        var messages = simulationContext.GetMessages(this);

        var newAirplanesForLand = messages
            .OfType<AskForLand>()
            .Select(message => message.Sender)
            .ToArray();

        return State with { AirplanesToLand = [..State.AirplanesToLand, ..newAirplanesForLand] };
    }
}

public record CoordinatorState(Airplane[] AirplanesToLand);

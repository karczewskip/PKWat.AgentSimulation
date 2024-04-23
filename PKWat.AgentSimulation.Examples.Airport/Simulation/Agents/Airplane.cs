namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Messages;

public class Airplane : SimulationAgent<AirportEnvironment, AirplaneState>
{
    protected override AirplaneState GetInitialState(ISimulationContext<AirportEnvironment> simulationContext)
    {
        return new AirplaneState(false);
    }

    protected override AirplaneState GetNextState(ISimulationContext<AirportEnvironment> simulationContext)
    {
        var coordinator = simulationContext.GetRequiredAgent<Coordinator>();

        if (!State.AskedForLand)
        {
            simulationContext.SendMessage(new AddressedAgentMessage(coordinator, new AskForLand(this)));

            return State with { AskedForLand = true };
        }

        return State;
    }
}

public record AirplaneState(bool AskedForLand);

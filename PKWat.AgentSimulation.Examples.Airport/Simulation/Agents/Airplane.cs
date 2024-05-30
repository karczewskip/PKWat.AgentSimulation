namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Messages;

public class Airplane : SimulationAgent<AirportEnvironment, AirplaneState>
{
    protected override AirplaneState GetNextState(IDictionary<Type, IPercept> percepts)
    {
        //var coordinator = simulationContext.GetRequiredAgent<Coordinator>();

        //if (!State.AskedForLand)
        //{
        //    simulationContext.SendMessage(new AddressedAgentMessage(coordinator, new AskForLand(this)));

        //    return State with { AskedForLand = true };
        //}

        //if(State.plannedLanding )

        return State;
    }


}

public record AirplaneState(
    bool AskedForLand = false,
    bool StartedLanding = false,
    TimeSpan? StartedLandingTime = null,
    TimeSpan? PlannedLandingTime = null);

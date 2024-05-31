namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core;

public class Coordinator : SimulationAgent<AirportEnvironment, CoordinatorState>
{
    protected override CoordinatorState GetInitialState(AirportEnvironment environment)
    {
        return new CoordinatorState(AgentId.Empty);
    }

    protected override CoordinatorState GetNextState(AirportEnvironment environment, SimulationTime simulationTime)
    {
        if(environment.IsLandingAirplane == false && environment.AirplanesAskingForLand.Length > 0)
        {
            return State with { AllowedAirplaneForLanding = environment.AirplanesAskingForLand[0] };
        }

        return State;
    }
}

public record CoordinatorState(AgentId AllowedAirplaneForLanding);
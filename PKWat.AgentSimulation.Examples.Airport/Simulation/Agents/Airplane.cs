namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Messages;

public class Airplane : SimulationAgent<AirportEnvironment, AirplaneState>
{
    protected override AirplaneState GetInitialState(AirportEnvironment environment)
    {
        return new AirplaneState();
    }

    protected override AirplaneState GetNextState(AirportEnvironment environment, SimulationTime simulationTime)
    {
        if (!State.AskedForLand)
        {
            return State with { AskedForLand = true, AskingForLand = true };
        }

        //if(State.AskingForLand && environment.AllowedForLand == Id)
        //{
        //    return State with { AskingForLand = false, Landing = true };
        //}

        return State;
    }
}

public record AirplaneState(
    bool AskedForLand = false,
    bool AskingForLand = false);

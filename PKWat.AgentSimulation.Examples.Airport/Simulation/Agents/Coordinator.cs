namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core;

public class Coordinator : SimulationAgent<AirportEnvironment, CoordinatorState>
{
    protected override CoordinatorState GetInitialState(AirportEnvironment environment)
    {
        return new CoordinatorState([]);
    }

    protected override CoordinatorState GetNextState(AirportEnvironment environment, SimulationTime simulationTime)
    {
        //var messages = simulationContext.GetMessages(this);

        //var newAirplanesForLand = messages
        //    .OfType<AskForLand>()
        //    .Select(message => message.Sender)
        //    .ToArray();

        //return State with { AirplanesToLand = [..State.AirplanesToLand, ..newAirplanesForLand] };
        return State;
    }
}

public record CoordinatorState(Airplane[] AirplanesToLand);

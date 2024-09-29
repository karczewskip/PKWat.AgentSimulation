namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core;
using System.Text.Json;

public class Coordinator : SimulationAgent<AirportEnvironment, CoordinatorState>
{
    protected override CoordinatorState GetInitialState(AirportEnvironment environment)
    {
        return new CoordinatorState(new Dictionary<AgentId, int>());
    }

    protected override CoordinatorState GetNextState(AirportEnvironment environment, SimulationTime simulationTime)
    {
        var airplanesAskingForLand = new Queue<AgentId>(environment.AirplanesAskingForLand);
        var busyLandingLines = environment.LandingAirplanes.Values.Union(environment.LandedAirplanes.Values).Union(environment.AllowedForLand.Values);
        var availableLandingLines = new Queue<int>(environment.AllLandingLines.Except(busyLandingLines));

        var newAllowedAirplanesForLanding = new Dictionary<AgentId, int>();
        while (airplanesAskingForLand.Any() && availableLandingLines.Any())
        {
            var nextAirplane = airplanesAskingForLand.Dequeue();
            var nextLine = availableLandingLines.Dequeue();
            newAllowedAirplanesForLanding.Add(nextAirplane, nextLine);
        }

        return State with { AllowedAirplanesForLanding = newAllowedAirplanesForLanding };
    }

    public override string CreateSnapshot()
    {
        return JsonSerializer.Serialize(State.AllowedAirplanesForLanding.Select(x => (x.Key, x.Value)).ToArray());
    
    }
}

public record CoordinatorState(IReadOnlyDictionary<AgentId, int> AllowedAirplanesForLanding);
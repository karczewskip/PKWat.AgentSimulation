namespace PKWat.AgentSimulation.Examples.Airport.Simulation;

using PKWat.AgentSimulation.Core;

public record AirportEnvironmentState(
    int[] AllLandingLines,
    AgentId[] AirplanesAskingForLand,
    IReadOnlyDictionary<AgentId, int> LandingAirplanes,
    IReadOnlyDictionary<AgentId, int> AllowedForLand,
    IReadOnlyDictionary<AgentId, int> LandedAirplanes,
    IReadOnlyDictionary<AgentId, AgentId[]> PassengersInEachAirplane)
{
    public static AirportEnvironmentState CreateUsingNumberOfLandingLines(int numberOfLandingLines)
        => new AirportEnvironmentState(
            Enumerable.Range(1, numberOfLandingLines).ToArray(),
            new AgentId[0],
            new Dictionary<AgentId, int>(),
            new Dictionary<AgentId, int>(),
            new Dictionary<AgentId, int>(),
            new Dictionary<AgentId, AgentId[]>());
}

public class AirportEnvironment : DefaultSimulationEnvironment<AirportEnvironmentState>
{
    public void SetAirplanesAskingForLand(AgentId[] airplanesAskingForLand) 
        => LoadState(
            GetState() with { AirplanesAskingForLand = airplanesAskingForLand });

    public void SetAllowedForLand(IReadOnlyDictionary<AgentId, int> allowedForLand)
        => LoadState(
            GetState() with { AllowedForLand = allowedForLand });

    public void SetLandingAirplane(IReadOnlyDictionary<AgentId, int> landingAirplanes)
        => LoadState(
            GetState() with { LandingAirplanes = landingAirplanes });

    public void SetLandedAirplanes(IReadOnlyDictionary<AgentId, int> landedAirplanes)
        => LoadState(
            GetState() with { LandedAirplanes = landedAirplanes });

    public void SetPassengersInEachAirplane(IReadOnlyDictionary<AgentId, AgentId[]> passengersInEachAirplane)
        => LoadState(
            GetState() with { PassengersInEachAirplane = passengersInEachAirplane });

    public bool NoPassengerInAirplane(AgentId airplaneId)
    {
        var state = GetState();
        return state.PassengersInEachAirplane.ContainsKey(airplaneId) == false 
            || state.PassengersInEachAirplane[airplaneId].Length == 0;
    }

    public int? GetAssignedLine(AgentId id)
    {
        var state = GetState();
        if(state.AllowedForLand.TryGetValue(id, out var line))
        {
            return line;
        }

        return null;
    }

    public bool AirplaneLanded(AgentId airplaneId)
    {
        var state = GetState();
        return state.LandedAirplanes.ContainsKey(airplaneId);
    }

    internal bool PassengerAllowedToCheckout(AgentId passengerId, AgentId airplaneId)
    {
        var state = GetState();
        return state.PassengersInEachAirplane[airplaneId].First() == passengerId;
    }

    public override SimulationCrashResult CheckCrashConditions()
    {
        if (GetState()
                .LandingAirplanes
                .GroupBy(x => x.Value)
                .Any(x => x.Count() > 1))
        {
            return SimulationCrashResult.Crash("More than one landing airplanes on the same line.");
        }

        if (GetState()
            .LandedAirplanes
            .GroupBy(x => x.Value)
            .Any(x => x.Count() > 1))
        {
            return SimulationCrashResult.Crash("More than one landed airplanes on the same line.");
        }

        if (GetState()
            .AllowedForLand
            .GroupBy(x => x.Value)
            .Any(x => x.Count() > 1))
        {
            return SimulationCrashResult.Crash("More than one allowed airplanes on the same line.");
        }

        return SimulationCrashResult.NoCrash;
    }

    public override object CreateSnapshot()
    {
        var state = GetState();
        return new
        {
            state.AllLandingLines,
            state.AirplanesAskingForLand,
            LandingAirplanes = state.LandingAirplanes.Select(x => new { Airplane = x.Key.Id, Line = x.Value }).ToArray(),
            AllowedForLand = state.AllowedForLand.Select(x => new { Airplane = x.Key.Id, Line = x.Value }).ToArray(),
            LandedAirplanes = state.LandedAirplanes.Select(x => new { Airplane = x.Key.Id, Line = x.Value }).ToArray(),
            PassengersInEachAirplane = state.PassengersInEachAirplane.Select(x => new { Airplane = x.Key.Id, Line = x.Value }).ToArray()
        };
    }
}

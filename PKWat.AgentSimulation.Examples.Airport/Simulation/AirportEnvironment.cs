namespace PKWat.AgentSimulation.Examples.Airport.Simulation;

using PKWat.AgentSimulation.Core;

public class AirportEnvironment : ISimulationEnvironment
{
    public AirportEnvironment(int numberOfLandingLines)
    {
        AllLandingLines = Enumerable.Range(1, numberOfLandingLines).ToArray();
    }

    public int[] AllLandingLines { get; private set; }
    public AgentId[] AirplanesAskingForLand { get; private set; } = [];
    public IReadOnlyDictionary<AgentId, int> LandingAirplanes { get; private set; } = new Dictionary<AgentId, int>();
    public IReadOnlyDictionary<AgentId, int> AllowedForLand { get; private set; } = new Dictionary<AgentId, int>();
    public IReadOnlyDictionary<AgentId, int> LandedAirplanes { get; private set; } = new Dictionary<AgentId, int>();
    public IReadOnlyDictionary<AgentId, AgentId[]> PassengersInEachAirplane { get; private set; } = new Dictionary<AgentId, AgentId[]>();

    public void SetAirplanesAskingForLand(AgentId[] airplanesAskingForLand)
    {
        AirplanesAskingForLand = airplanesAskingForLand;
    }

    public void SetAllowedForLand(IReadOnlyDictionary<AgentId, int> allowedForLand)
    {
        AllowedForLand = allowedForLand;
    }

    public void SetLandingAirplane(IReadOnlyDictionary<AgentId, int> landingAirplanes)
    {
        LandingAirplanes = landingAirplanes;
    }

    public void SetLandedAirplanes(IReadOnlyDictionary<AgentId, int> landedAirplanes)
    {
        LandedAirplanes = landedAirplanes;
    }

    public void SetPassengersInEachAirplane(IReadOnlyDictionary<AgentId, AgentId[]> passengersInEachAirplane)
    {
        PassengersInEachAirplane = passengersInEachAirplane;
    }

    public bool NoPassengerInAirplane(AgentId airplaneId)
    {
        return PassengersInEachAirplane.ContainsKey(airplaneId) == false || PassengersInEachAirplane[airplaneId].Length == 0;
    }

    public int? GetAssignedLine(AgentId id)
    {
        if(AllowedForLand.TryGetValue(id, out var line))
        {
            return line;
        }

        return null;
    }

    public bool AirplaneLanded(AgentId airplaneId)
    {
        return LandedAirplanes.ContainsKey(airplaneId);
    }

    internal bool PassengerAllowedToCheckout(AgentId passengerId, AgentId airplaneId)
    {
        return PassengersInEachAirplane[airplaneId].First() == passengerId;
    }

    public object CreateSnapshot()
    {
        return new
        {
            AllLandingLines,
            AirplanesAskingForLand,
            LandingAirplanes = LandingAirplanes.Select(x => (x.Key, x.Value)).ToArray(),
            AllowedForLand = AllowedForLand.Select(x => (x.Key, x.Value)).ToArray(),
            LandedAirplanes = LandedAirplanes.Select(x => (x.Key, x.Value)).ToArray(),
            PassengersInEachAirplane = PassengersInEachAirplane.Select(x => (x.Key, x.Value)).ToArray()
        };
    }
}

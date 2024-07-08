namespace PKWat.AgentSimulation.Examples.Airport.Simulation;

using PKWat.AgentSimulation.Core;
using System;

public class AirportEnvironment
{
    public int[] AllLandingLines { get; private set; } = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    public AgentId[] AirplanesAskingForLand { get; private set; } = [];
    public IReadOnlyDictionary<AgentId, int> LandingAirplanes { get; private set; } = new Dictionary<AgentId, int>();
    public IReadOnlyDictionary<AgentId, int> AllowedForLand { get; private set; } = new Dictionary<AgentId, int>();

    public void SetAirplanesAskingForLand(AgentId[] airplanesAskingForLand)
    {
        AirplanesAskingForLand = airplanesAskingForLand;
    }

    public void SetLandingAirplane(IReadOnlyDictionary<AgentId, int> landingAirplanes)
    {
        LandingAirplanes = landingAirplanes;
    }

    public void SetAllowedForLand(IReadOnlyDictionary<AgentId, int> allowedForLand)
    {
        AllowedForLand = allowedForLand;
    }

    public bool NoPassengerInAirplane(AgentId airplaneId)
    {
        return true;
    }

    internal int? GetAssignedLine(AgentId id)
    {
        if(AllowedForLand.TryGetValue(id, out var line))
        {
            return line;
        }

        return null;
    }
}

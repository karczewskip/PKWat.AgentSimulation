namespace PKWat.AgentSimulation.Examples.Airport.Simulation;

using PKWat.AgentSimulation.Core;

public class AirportEnvironment
{
    public bool IsLandingAirplane => LandingAirplane != AgentId.Empty;

    public AgentId[] AirplanesAskingForLand { get; private set; } = [];
    public AgentId LandingAirplane { get; private set; } = AgentId.Empty;
    public AgentId AllowedForLand { get; private set; } = AgentId.Empty;

    public void SetAirplanesAskingForLand(AgentId[] airplanesAskingForLand)
    {
        AirplanesAskingForLand = airplanesAskingForLand;
    }

    public void SetLandingAirplane(AgentId airplaneId)
    {
        LandingAirplane = airplaneId;
    }

    public void SetAllowedForLand(AgentId allowedForLand)
    {
        AllowedForLand = allowedForLand;
    }

}

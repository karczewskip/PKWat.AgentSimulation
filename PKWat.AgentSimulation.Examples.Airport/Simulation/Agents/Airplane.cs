namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core.Agent;

public class Airplane : SimpleSimulationAgent<AirportEnvironment>
{
    public int? LandingLine { get; set; }
    public TimeSpan? StartedLandingTime { get; private set; }
    public TimeSpan? PlannedFinishedLandingTime { get; private set; }

    public TimeSpan? StartedDepartureTime { get; private set; }
    public TimeSpan? PlannedFinishedDepartureTime { get; private set; }

    public List<AgentId> Passengers { get; } = new();

    public bool WaitsForLanding => !StartedLandingTime.HasValue;

    public void StartLanding(TimeSpan landingTime, TimeSpan plannedFinishLandingTime)
    {
        StartedLandingTime = landingTime;
        PlannedFinishedLandingTime = plannedFinishLandingTime;
    }

    internal bool IsLanding(TimeSpan now)
        => StartedLandingTime.HasValue && now >= StartedLandingTime;

    public bool IsLanded(TimeSpan time)
        => PlannedFinishedLandingTime.HasValue && time >= PlannedFinishedLandingTime;

    public void StartDeparture(TimeSpan departureTime, TimeSpan plannedFinishDepartureTime)
    {
        StartedDepartureTime = departureTime;
        PlannedFinishedDepartureTime = plannedFinishDepartureTime;
    }

    internal bool IsDeparting(TimeSpan now)
        => StartedDepartureTime.HasValue && now >= StartedDepartureTime && now <= PlannedFinishedDepartureTime;

    public bool IsAway(TimeSpan time)
        => PlannedFinishedDepartureTime.HasValue && time >= PlannedFinishedDepartureTime;
}

using PKWat.AgentSimulation.Core.Agent;

namespace PKWat.AgentSimulation.Examples.Airport2.Agents;

public class Airplane : SimpleSimulationAgent
{
    public int? AssignedLine { get; set; }
    public TimeSpan? StartedLandingTime { get; private set; }
    public TimeSpan? PlannedFinishedLandingTime { get; private set; }

    public TimeSpan? StartedDepartureTime { get; private set; }
    public TimeSpan? PlannedFinishedDepartureTime { get; private set; }

    public bool IsBeforeDeparture => !StartedDepartureTime.HasValue;

    public Queue<AgentId> PassengersInAirplane { get; } = new();
    public TimeSpan? PassengerCheckoutBlockTime { get; set; }

    public bool WaitsForLanding => !StartedLandingTime.HasValue;

    public void StartLanding(TimeSpan landingTime, TimeSpan plannedFinishLandingTime)
    {
        StartedLandingTime = landingTime;
        PlannedFinishedLandingTime = plannedFinishLandingTime;
    }

    public bool IsLanding(TimeSpan now)
        => StartedLandingTime.HasValue && now >= StartedLandingTime && now < PlannedFinishedLandingTime;

    public bool IsLanded(TimeSpan time)
        => PlannedFinishedLandingTime.HasValue && time >= PlannedFinishedLandingTime;

    public void StartDeparture(TimeSpan departureTime, TimeSpan plannedFinishDepartureTime)
    {
        StartedDepartureTime = departureTime;
        PlannedFinishedDepartureTime = plannedFinishDepartureTime;
    }

    public bool IsDeparting(TimeSpan now)
        => StartedDepartureTime.HasValue && now >= StartedDepartureTime && now <= PlannedFinishedDepartureTime;

    public bool IsAway(TimeSpan time)
        => PlannedFinishedDepartureTime.HasValue && time >= PlannedFinishedDepartureTime;

    public bool WaitsForPassangersCheckout(TimeSpan now)
        => IsLanded(now)
        && (PassengersInAirplane.Any()
            || PassengerCheckoutBlockTime.HasValue && now < PassengerCheckoutBlockTime);

    internal bool IsDepartured(TimeSpan time)
        => PlannedFinishedDepartureTime.HasValue && time > PlannedFinishedDepartureTime;

    override public object CreateSnapshot()
    {
        return new
        {
            Id,
            AssignedLine
        };
    }
}

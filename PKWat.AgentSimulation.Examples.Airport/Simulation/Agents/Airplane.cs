namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core.Agent;

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

    internal bool IsLanding(TimeSpan now)
        => StartedLandingTime.HasValue && now >= StartedLandingTime && now < PlannedFinishedLandingTime;

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

    internal bool WaitsForPassangersCheckout(TimeSpan now)
        => IsLanded(now) 
        && (PassengersInAirplane.Any() 
            || (PassengerCheckoutBlockTime.HasValue && now < PassengerCheckoutBlockTime));

    internal bool IsDepartured(TimeSpan time)
        => PlannedFinishedDepartureTime.HasValue && time > PlannedFinishedDepartureTime;
}

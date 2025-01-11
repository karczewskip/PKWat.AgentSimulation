namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core.Agent;
using System;

public class Passenger : SimpleSimulationAgent<AirportEnvironment>
{
    public AgentId AirplaneId { get; private set; }
    public TimeSpan? StartedCheckoutTime { get; private set; }
    public TimeSpan? EndPlannedCheckoutTime { get; private set; }

    public void SetAirplane(AgentId airplaneId)
    {
        AirplaneId = airplaneId;
    }

    public bool IsBeforeCheckout => StartedCheckoutTime == null;

    public void StartCheckout(TimeSpan startCheckout, TimeSpan plannedEndCheckout)
    {
        StartedCheckoutTime = startCheckout;
        EndPlannedCheckoutTime = plannedEndCheckout;
    }

    public bool IsCheckouting(TimeSpan now)
        => StartedCheckoutTime.HasValue && StartedCheckoutTime.Value <= now && EndPlannedCheckoutTime.Value > now;

    public bool IsCheckouted(TimeSpan now) 
        => EndPlannedCheckoutTime.HasValue && EndPlannedCheckoutTime.Value <= now;
}

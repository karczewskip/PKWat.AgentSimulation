using PKWat.AgentSimulation.Core.Agent;

namespace PKWat.AgentSimulation.Examples.Airport2.Agents;


public class Passenger : SimpleSimulationAgent
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

    public override object CreateSnapshot()
    {
        return new
        {
            Id,
            AirplaneId,
            StartedCheckoutTime,
            EndPlannedCheckoutTime
        };
    }
}

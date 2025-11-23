using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Core.Time;
using PKWat.AgentSimulation.Examples.Airport2.Agents;

namespace PKWat.AgentSimulation.Examples.Airport2.Stages;

public class StartDeparture(ISimulationCalendarScheduler simulationCalendarScheduler) : ISimulationStage
{
    private TimeSpan departureTime = TimeSpan.FromMinutes(10);

    public void SetDepartureTime(TimeSpan departureTime)
    {
        this.departureTime = departureTime;
    }

    public async Task Execute(ISimulationContext context)
    {
        foreach (var airplane in context.GetAgents<Airplane>().Where(
            x => x.IsLanded(context.Time.Time)
            && x.PassengersInAirplane.Any() == false
            && x.IsBeforeDeparture))
        {
            var startDepartureTime = context.Time.Time;
            var endDepartureTime = startDepartureTime + departureTime;
            airplane.StartDeparture(startDepartureTime, endDepartureTime);

            simulationCalendarScheduler.ScheduleNewStepAt(endDepartureTime);
        }
    }
}

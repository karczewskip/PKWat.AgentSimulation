namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Agents;
using System;
using System.Linq;
using System.Threading.Tasks;

public class StartDeparture : ISimulationStage
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
        }
    }
}

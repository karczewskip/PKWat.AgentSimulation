namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;
using System;
using System.Linq;
using System.Threading.Tasks;

internal class StartDeparture : ISimulationStage<AirportEnvironment>
{
    private TimeSpan departureTime = TimeSpan.FromMinutes(10);

    public void SetDepartureTime(TimeSpan departureTime)
    {
        this.departureTime = departureTime;
    }

    public async Task Execute(ISimulationContext<AirportEnvironment> context)
    {
        foreach (var airplane in context.GetAgents<Airplane>().Where(
            x => x.IsLanded(context.SimulationTime.Time) 
            && x.PassengersInAirplane.Any() == false
            && x.IsBeforeDeparture))
        {
            var startDepartureTime = context.SimulationTime.Time;
            var endDepartureTime = startDepartureTime + departureTime;
            airplane.StartDeparture(startDepartureTime, endDepartureTime);
        }
    }
}

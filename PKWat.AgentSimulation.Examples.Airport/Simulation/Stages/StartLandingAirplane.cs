namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;
using System;
using System.Linq;
using System.Threading.Tasks;

internal class StartLandingAirplane : ISimulationStage<AirportEnvironment>
{
    private TimeSpan landingTime = TimeSpan.FromMinutes(10);

    public void SetLandingTime(TimeSpan landingTime)
    {
        this.landingTime = landingTime;
    }

    public async Task Execute(ISimulationContext<AirportEnvironment> context)
    {
        var airplanes = context.GetAgents<Airplane>().Where(x => x.WaitsForLanding && x.LandingLine.HasValue);
        foreach (var airplane in airplanes)
        {
            var start = context.SimulationTime.Time;
            var end = start + landingTime;
            airplane.StartLanding(start, end);
        }
    }
}

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Agents;
using System;
using System.Linq;
using System.Threading.Tasks;

internal class StartLandingAirplane : ISimulationStage
{
    private TimeSpan landingTime = TimeSpan.FromMinutes(10);

    public void SetLandingTime(TimeSpan landingTime)
    {
        this.landingTime = landingTime;
    }

    public async Task Execute(ISimulationContext context)
    {
        var airplanes = context.GetAgents<Airplane>().Where(x => x.WaitsForLanding && x.AssignedLine.HasValue);
        foreach (var airplane in airplanes)
        {
            var start = context.Time.Time;
            var end = start + landingTime;
            airplane.StartLanding(start, end);
        }
    }
}

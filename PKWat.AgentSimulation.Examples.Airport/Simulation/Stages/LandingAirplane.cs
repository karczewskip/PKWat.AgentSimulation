namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class LandingAirplane : ISimulationStage<AirportEnvironment>
{
    public Task Execute(ISimulationContext<AirportEnvironment> context)
    {
        //var airplanes = context.GetAgents<Airplane>().Where(a => a);
        //foreach (var airplane in airplanes)
        //{
        //    if (airplane.Coordinates.Y == 0)
        //    {
        //        airplane.Land();
        //    }
        //}
        return Task.CompletedTask;
    }
}

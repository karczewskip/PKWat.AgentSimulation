using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKWat.AgentSimulation.Examples.LangtonAnts.Stages;

public class CalculateOccupiedGrid : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<LangtonAntsEnvironment>();
        environment.ClearOccupiedGrid();
        var ants = context.GetAgents<Ant>();

        foreach (var ant in ants)
        {
            var (antX, antY) = ant.GetCurrentPosition();
            environment.MarkOccupied(antX, antY);
        }
    }
}

namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
using System.Collections.Generic;
using System.Threading.Tasks;

internal class AddAntHills : ISimulationStage
{
    private List<AntHill> antHills = new List<AntHill>();

    public void AddAntHill(AntHill antHill)
    {
        antHills.Add(antHill);
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<ColonyEnvironment>();

        foreach (var antHill in antHills)
        {
            environment.AntHills.Add(antHill);
        }
    }
}

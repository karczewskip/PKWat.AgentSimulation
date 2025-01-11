namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
using System.Collections.Generic;
using System.Threading.Tasks;

internal class AddAntHills : ISimulationStage<ColonyEnvironment>
{
    private List<AntHill> antHills = new List<AntHill>();

    public void AddAntHill(AntHill antHill)
    {
        antHills.Add(antHill);
    }

    public async Task Execute(ISimulationContext<ColonyEnvironment> context)
    {
        foreach (var antHill in antHills)
        {
            context.SimulationEnvironment.AntHills.Add(antHill);
        }
    }
}

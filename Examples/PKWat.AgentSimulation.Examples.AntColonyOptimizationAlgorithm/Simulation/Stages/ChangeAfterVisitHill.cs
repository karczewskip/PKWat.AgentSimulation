namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
using System.Threading.Tasks;

internal class ChangeAfterVisitHill : ISimulationStage<ColonyEnvironment>
{
    public async Task Execute(ISimulationContext<ColonyEnvironment> context)
    {
        var hills = context.SimulationEnvironment.AntHills;

        foreach (var ant in context.GetAgents<Ant>())
        {
            foreach (var hill in hills)
            {
                if(ant.Coordinates.IsInRange(hill.Coordinates, hill.Size))
                {
                    ant.VisitHill();
                }
            }
        }
    }
}

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony;
using System.Threading.Tasks;

internal class ChangeAfterVisitHill : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<ColonyEnvironment>();

        var hills = environment.AntHills;

        foreach (var ant in context.GetAgents<Ant>())
        {
            foreach (var hill in hills)
            {
                if (ant.Coordinates.IsInRange(hill.Coordinates, hill.SizeRadius))
                {
                    ant.VisitHill();
                }
            }
        }
    }
}

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony;
using System.Threading.Tasks;

internal class DecreasePheromones : ISimulationStage
{
    private double decreaseRate = 0.95;

    public void SetDecreaseRate(double decreaseRate)
    {
        this.decreaseRate = decreaseRate;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<ColonyEnvironment>();

        for (var x = 0; x < environment.Width; x++)
        {
            for (var y = 0; y < environment.Height; y++)
            {
                var pheromones = environment.Pheromones[x, y];
                pheromones.Decrease(decreaseRate);
            }
        }
    }
}

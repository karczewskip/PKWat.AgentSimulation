namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using System.Threading.Tasks;

internal class DecreasePheromones : ISimulationStage<ColonyEnvironment>
{
    private double decreaseRate = 0.95;

    public void SetDecreaseRate(double decreaseRate)
    {
        this.decreaseRate = decreaseRate;
    }

    public async Task Execute(ISimulationContext<ColonyEnvironment> context)
    {
        for (var x = 0; x < context.SimulationEnvironment.Width; x++)
        {
            for (var y = 0; y < context.SimulationEnvironment.Height; y++)
            {
                var pheromones = context.SimulationEnvironment.Pheromones[x, y];
                pheromones.Decrease(decreaseRate);
            }
        }
    }
}

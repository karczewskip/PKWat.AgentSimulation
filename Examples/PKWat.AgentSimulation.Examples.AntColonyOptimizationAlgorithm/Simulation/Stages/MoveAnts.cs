namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
using System.Threading.Tasks;

internal class MoveAnts(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage<ColonyEnvironment>
{
    public async Task Execute(ISimulationContext<ColonyEnvironment> context)
    {
        foreach (var ant in context.GetAgents<Ant>())
        {
            var coordinates = ant.Coordinates;
            var x = coordinates.X + randomNumbersGenerator.Next(3) - 1;
            var y = coordinates.Y + randomNumbersGenerator.Next(3) - 1;

            if (x < 0 || x >= context.SimulationEnvironment.Width)
            {
                x = coordinates.X;
            }

            if (y < 0 || y >= context.SimulationEnvironment.Height)
            {
                y = coordinates.Y;
            }

            coordinates.SetCoordinates(x, y);
            ant.PathLength++;
        }
    }
}
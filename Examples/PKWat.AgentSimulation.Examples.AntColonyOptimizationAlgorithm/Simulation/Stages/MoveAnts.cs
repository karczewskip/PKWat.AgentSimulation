namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
using System.Threading.Tasks;

internal class MoveAnts(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage<ColonyEnvironment>
{
    private readonly Dictionary<ColonyDirection, ColonyDirection[]> possibleDirections = new Dictionary<ColonyDirection, ColonyDirection[]>
    {
        { ColonyDirection.None, new[] { ColonyDirection.Up, ColonyDirection.Down, ColonyDirection.Left, ColonyDirection.Right } },
        { ColonyDirection.Up, new[] { ColonyDirection.Up, ColonyDirection.Up, ColonyDirection.UpLeft, ColonyDirection.UpRight } },
        { ColonyDirection.UpRight, new[] { ColonyDirection.UpRight, ColonyDirection.UpRight, ColonyDirection.Up, ColonyDirection.Right } },
        { ColonyDirection.Right, new[] { ColonyDirection.Right, ColonyDirection.Right, ColonyDirection.UpRight, ColonyDirection.DownRight } },
        { ColonyDirection.DownRight, new[] { ColonyDirection.DownRight, ColonyDirection.DownRight, ColonyDirection.Right, ColonyDirection.Down } },
        { ColonyDirection.Down, new[] { ColonyDirection.Down, ColonyDirection.Down, ColonyDirection.DownRight, ColonyDirection.DownLeft } },
        { ColonyDirection.DownLeft, new[] { ColonyDirection.DownLeft, ColonyDirection.DownLeft, ColonyDirection.Down, ColonyDirection.Left } },
        { ColonyDirection.Left, new[] { ColonyDirection.Left, ColonyDirection.Left, ColonyDirection.DownLeft, ColonyDirection.UpLeft } },
        { ColonyDirection.UpLeft, new[] { ColonyDirection.UpLeft, ColonyDirection.UpLeft, ColonyDirection.Left, ColonyDirection.Up } }
    };

    public async Task Execute(ISimulationContext<ColonyEnvironment> context)
    {
        foreach (var ant in context.GetAgents<Ant>())
        {
            var coordinates = ant.Coordinates;
            var newDirection = GetNewDirection(ant.Direction);

            coordinates.MoveBy(newDirection, context.SimulationEnvironment.Width - 1, context.SimulationEnvironment.Height - 1);
            ant.PathLength++;
        }
    }

    private ColonyDirection GetNewDirection(ColonyDirection direction)
    {
        var directions = possibleDirections[direction];
        return directions[randomNumbersGenerator.Next(directions.Length)];
    }
}
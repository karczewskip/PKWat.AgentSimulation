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
            SetNewPosition(ant, context.SimulationEnvironment);
        }
    }

    private void SetNewPosition(Ant ant, ColonyEnvironment colonyEnvironment)
    {
        var possibleDirections = this.possibleDirections[ant.Direction];

        if (SetUsingPheromones(ant, colonyEnvironment, colonyEnvironment))
        {
            return;
        }

        SetNewRandomPostion(ant, colonyEnvironment, possibleDirections);
    }

    private void SetNewRandomPostion(Ant ant, ColonyEnvironment colonyEnvironment, ColonyDirection[] possibleDirections)
    {
        var newDirection = possibleDirections[randomNumbersGenerator.Next(possibleDirections.Length)];
        ant.Coordinates.MoveBy(newDirection, colonyEnvironment.Width - 1, colonyEnvironment.Height - 1);
        ant.Direction = newDirection;
        ant.PathLength++;
    }

    private bool SetUsingPheromones(Ant ant, ColonyEnvironment environment, ColonyEnvironment colonyEnvironment)
    {
        return false;
    }
}
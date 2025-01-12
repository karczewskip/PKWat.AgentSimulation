namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony;
using System.Threading.Tasks;

internal class MoveAnts(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage
{
    private double temperature = 0.01;

    private readonly Dictionary<ColonyDirection, ColonyDirection> verticalMirrorDirection = new Dictionary<ColonyDirection, ColonyDirection>
    {
        { ColonyDirection.Up, ColonyDirection.Down },
        { ColonyDirection.Down, ColonyDirection.Up },
        { ColonyDirection.UpRight, ColonyDirection.DownRight },
        { ColonyDirection.DownRight, ColonyDirection.UpRight },
        { ColonyDirection.DownLeft, ColonyDirection.UpLeft },
        { ColonyDirection.UpLeft, ColonyDirection.DownLeft }
    };

    private readonly Dictionary<ColonyDirection, ColonyDirection> horizontalMirrorDirection = new Dictionary<ColonyDirection, ColonyDirection>
    {
        { ColonyDirection.Left, ColonyDirection.Right },
        { ColonyDirection.Right, ColonyDirection.Left },
        { ColonyDirection.UpLeft, ColonyDirection.UpRight },
        { ColonyDirection.UpRight, ColonyDirection.UpLeft },
        { ColonyDirection.DownRight, ColonyDirection.DownLeft },
        { ColonyDirection.DownLeft, ColonyDirection.DownRight }
    };

    private readonly Dictionary<ColonyDirection, ColonyDirection> cornerMirrorDirection = new Dictionary<ColonyDirection, ColonyDirection>
    {
        { ColonyDirection.UpRight, ColonyDirection.DownLeft },
        { ColonyDirection.DownRight, ColonyDirection.UpLeft },
        { ColonyDirection.DownLeft, ColonyDirection.UpRight },
        { ColonyDirection.UpLeft, ColonyDirection.DownRight }
    };

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

    public void SetTemperature(double temperature)
    {
        this.temperature = temperature;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<ColonyEnvironment>();

        foreach (var ant in context.GetAgents<Ant>())
        {
            SetNewPosition(ant, environment);
        }
    }

    private void SetNewPosition(Ant ant, ColonyEnvironment colonyEnvironment)
    {
        var possibleDirections = this.possibleDirections[ant.Direction];

        if (SetForEdges(ant, colonyEnvironment))
        {
            return;
        }

        if (SetUsingPheromones(ant, colonyEnvironment, possibleDirections))
        {
            return;
        }

        SetNewRandomPostion(ant, colonyEnvironment, possibleDirections);
    }

    private bool SetForEdges(Ant ant, ColonyEnvironment colonyEnvironment)
    {
        var antCoordinates = ant.Coordinates;

        var leftEdge = antCoordinates.X == 0;
        var rightEdge = antCoordinates.X == colonyEnvironment.Width - 1;
        var topEdge = antCoordinates.Y == colonyEnvironment.Height - 1;
        var bottomEdge = antCoordinates.Y == 0;

        if (
            !leftEdge
            && !rightEdge
            && !topEdge
            && !bottomEdge)
        {
            return false;
        }

        // Corner Mirror
        if (
            (leftEdge || rightEdge) &&
            (topEdge || bottomEdge))
        {
            SetUsingDirection(ant, colonyEnvironment, cornerMirrorDirection[ant.Direction]);
            return true;
        }

        // Horizontal Mirror
        if (leftEdge || rightEdge)
        {
            SetUsingDirection(ant, colonyEnvironment, horizontalMirrorDirection[ant.Direction]);
            return true;
        }

        // Vertical Mirror
        if (topEdge || bottomEdge)
        {
            SetUsingDirection(ant, colonyEnvironment, verticalMirrorDirection[ant.Direction]);
            return true;
        }

        return true;
    }

    private bool SetUsingPheromones(Ant ant, ColonyEnvironment colonyEnvironment, ColonyDirection[] possibleDirections)
    {
        var antCoordinates = ant.Coordinates;

        if (antCoordinates.X <= 0 || antCoordinates.Y <= 0 || antCoordinates.X >= colonyEnvironment.Width - 1 || antCoordinates.Y >= colonyEnvironment.Height - 1)
        {
            return false;
        }

        if (randomNumbersGenerator.NextDouble() < temperature)
        {
            return false;
        }

        var possiblePheromones = possibleDirections.Distinct().Select(possibleDirection =>
        {
            var xForNewDirection = possibleDirection switch
            {
                ColonyDirection.Up => antCoordinates.X,
                ColonyDirection.Down => antCoordinates.X,
                ColonyDirection.Left => antCoordinates.X - 1,
                ColonyDirection.Right => antCoordinates.X + 1,
                ColonyDirection.UpLeft => antCoordinates.X - 1,
                ColonyDirection.UpRight => antCoordinates.X + 1,
                ColonyDirection.DownLeft => antCoordinates.X - 1,
                ColonyDirection.DownRight => antCoordinates.X + 1,
                _ => antCoordinates.X
            };

            var yForNewDirection = possibleDirection switch
            {
                ColonyDirection.Up => antCoordinates.Y + 1,
                ColonyDirection.Down => antCoordinates.Y - 1,
                ColonyDirection.Left => antCoordinates.Y,
                ColonyDirection.Right => antCoordinates.Y,
                ColonyDirection.UpLeft => antCoordinates.Y + 1,
                ColonyDirection.UpRight => antCoordinates.Y + 1,
                ColonyDirection.DownLeft => antCoordinates.Y - 1,
                ColonyDirection.DownRight => antCoordinates.Y - 1,
                _ => antCoordinates.Y
            };

            var pheromonesForNewDirection = colonyEnvironment.Pheromones[xForNewDirection, yForNewDirection];

            return (Direction: possibleDirection, Pheromones: pheromonesForNewDirection);
        });

        if (ant.IsCarryingFood)
        {
            var bestPossibility = possiblePheromones.Where(x => x.Pheromones.Home > 0.1).OrderByDescending(x => x.Pheromones.Home).FirstOrDefault();
            if (bestPossibility == default)
            {
                return false;
            }

            SetUsingDirection(ant, colonyEnvironment, bestPossibility.Direction);

            return true;
        }
        else
        {
            var bestPossibility = possiblePheromones.Where(x => x.Pheromones.Food > 0.1).OrderByDescending(x => x.Pheromones.Food).FirstOrDefault();
            if (bestPossibility == default)
            {
                return false;
            }

            SetUsingDirection(ant, colonyEnvironment, bestPossibility.Direction);

            return true;
        }
    }

    private void SetNewRandomPostion(Ant ant, ColonyEnvironment colonyEnvironment, ColonyDirection[] possibleDirections)
    {
        var newDirection = possibleDirections[randomNumbersGenerator.Next(possibleDirections.Length)];
        SetUsingDirection(ant, colonyEnvironment, newDirection);
    }

    private void SetUsingDirection(Ant ant, ColonyEnvironment colonyEnvironment, ColonyDirection newDirection)
    {
        ant.Coordinates.MoveBy(newDirection, colonyEnvironment.Width - 1, colonyEnvironment.Height - 1);
        ant.Direction = newDirection;
        ant.PathLength++;
    }
}
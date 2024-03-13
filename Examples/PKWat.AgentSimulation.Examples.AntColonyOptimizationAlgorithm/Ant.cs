namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm;

using PKWat.AgentSimulation.Core;
using System;

public class Ant : IAgent<ColonyEnvironment>
{
    private const int PheromonesStrengthInitialValue = 1000000;

    private readonly IRandomNumbersGenerator _randomNumbersGenerator;

    public ColonyDirection Direction { get; private set; }
    public ColonyCoordinates Coordinates { get; private set; }
    public bool IsCarryingFood { get; private set; } = false;
    public double PheromonesStrength => PheromonesStrengthInitialValue * Math.Pow(0.7, PathLength);
    public int PathLength { get; private set; }

    public Ant(IRandomNumbersGenerator randomNumbersGenerator)
    {
        _randomNumbersGenerator = randomNumbersGenerator;
    }

    public void Initialize(ISimulationContext<ColonyEnvironment> simulationContext)
    {
        var simulationEnvironment = simulationContext.SimulationEnvironment;
        Coordinates = simulationEnvironment.AntHill.Coordinates;
        Direction = ColonyDirection.Random(_randomNumbersGenerator);
        PathLength = 1;
    }

    public void Decide(ISimulationContext<ColonyEnvironment> simulationContext)
    {
        var simulationEnvironment = simulationContext.SimulationEnvironment;
        if (!IsCarryingFood && simulationEnvironment.FoodSource.Coordinates.DistanceFrom(Coordinates) <= simulationEnvironment.FoodSource.Size)
        {
            IsCarryingFood = true;
            PathLength = 1;
            Direction = Direction.Opposite();
        }
        else if (IsCarryingFood && simulationEnvironment.AntHill.Coordinates.DistanceFrom(Coordinates) <= simulationEnvironment.AntHill.Size)
        {
            IsCarryingFood = false;
            PathLength = 1;
            Direction = Direction.Opposite();
        }
        else
        {
            var possibleDirections = ColonyDirection.GeneratePossibleDirections(Direction);

            if(_randomNumbersGenerator.Next(1000) < 1)
            {
                Direction = possibleDirections[_randomNumbersGenerator.Next(possibleDirections.Length)];
            }
            else
            {
                var consideringDirections = new List<ColonyDirection>() { possibleDirections[0] };
                var pheromonesStrength = GetPheromonesStrength(simulationEnvironment, Coordinates.MovedBy(possibleDirections[0]));
                for (int i = 1; i < possibleDirections.Length; i++)
                {
                    var consideringCoordinates = Coordinates.MovedBy(possibleDirections[i]);
                    var consideringPheromonesStrength = GetPheromonesStrength(simulationEnvironment, consideringCoordinates);
                    if (consideringPheromonesStrength > pheromonesStrength)
                    {
                        consideringDirections.Clear();
                        consideringDirections.Add(possibleDirections[i]);
                        pheromonesStrength = consideringPheromonesStrength;
                    }
                    else if (consideringPheromonesStrength == pheromonesStrength)
                    {
                        consideringDirections.Add(possibleDirections[i]);
                    }
                }

                Direction = consideringDirections[_randomNumbersGenerator.Next(consideringDirections.Count)];
            }

        }
    }

    private double GetPheromonesStrength(ColonyEnvironment simulationEnvironment, ColonyCoordinates coordinates)
    {
        var pheromones = simulationEnvironment.GetPheromones(coordinates);
        return IsCarryingFood ? pheromones.Home : pheromones.Food;
    }

    public void Act(ISimulationContext<ColonyEnvironment> simulationContext)
    {
        var simulationEnvironment = simulationContext.SimulationEnvironment;
        if (IsCarryingFood)
        {
            simulationEnvironment.AddFoodPheromones(Coordinates, PheromonesStrength);
        }
        else
        {
            simulationEnvironment.AddHomePheromones(Coordinates, PheromonesStrength);
        }

        var consideringCoordinates = Coordinates.MovedBy(Direction);
        if(simulationEnvironment.IsInBounds(consideringCoordinates))
        {
            Coordinates = consideringCoordinates;
        }

        PathLength += 1;
    }
}

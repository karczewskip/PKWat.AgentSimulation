namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm;

using PKWat.AgentSimulation.Core;
using System;

public class Ant : IAgent<ColonyEnvironment>
{
    private const int PheromonesStrengthInitialValue = 2;
    public ColonyDirection Direction { get; private set; }
    public ColonyCoordinates Coordinates { get; private set; }
    public bool IsCarryingFood { get; private set; } = false;
    public double PheromonesStrength { get; private set; }

    public void Initialize(ColonyEnvironment simulationEnvironment)
    {
        Coordinates = simulationEnvironment.AntHill.Coordinates;
        Direction = ColonyDirection.Random();
        PheromonesStrength = PheromonesStrengthInitialValue;
    }

    public void Decide(ColonyEnvironment simulationEnvironment)
    {
        if (!IsCarryingFood && simulationEnvironment.FoodSource.Coordinates.DistanceFrom(Coordinates) <= simulationEnvironment.FoodSource.Size)
        {
            IsCarryingFood = true;
            PheromonesStrength = PheromonesStrengthInitialValue;
            Direction = Direction.Opposite();
        }
        else if (IsCarryingFood && simulationEnvironment.AntHill.Coordinates.DistanceFrom(Coordinates) <= simulationEnvironment.AntHill.Size)
        {
            IsCarryingFood = false;
            PheromonesStrength = PheromonesStrengthInitialValue;
            Direction = Direction.Opposite();
        }
        else
        {
            var possibleDirections = ColonyDirection.GeneratePossibleDirections(Direction);

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

            var random = new Random();
            Direction = consideringDirections[random.Next(consideringDirections.Count)];
        }
    }

    private double GetPheromonesStrength(ColonyEnvironment simulationEnvironment, ColonyCoordinates coordinates)
    {
        var pheromones = simulationEnvironment.GetPheromones(coordinates);
        return IsCarryingFood ? pheromones.Home : pheromones.Food;
    }

    public void Act(ColonyEnvironment simulationEnvironment)
    {
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

        PheromonesStrength *= 0.95;
    }
}

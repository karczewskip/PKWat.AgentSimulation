namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm;

using PKWat.AgentSimulation.Core;
using System;

public record AntState(ColonyDirection Direction, ColonyCoordinates Coordinates, bool IsCarryingFood, int PathLength)
{
    private const int PheromonesStrengthInitialValue = 1000000;
    public double PheromonesStrength => PheromonesStrengthInitialValue * Math.Pow(0.7, PathLength);
}

public class Ant(IRandomNumbersGenerator randomNumbersGenerator) : SimulationAgent<ColonyEnvironment, AntState>
{
    protected override AntState GetNextState(IPercept[] percepts)
    {
        var simulationEnvironment = simulationContext.SimulationEnvironment;
        if (!State.IsCarryingFood && simulationEnvironment.FoodSource.Coordinates.DistanceFrom(State.Coordinates) <= simulationEnvironment.FoodSource.Size)
        {
            return State with
            {
                IsCarryingFood = true,
                PathLength = 1,
                Direction = State.Direction.Opposite()
            };
        }
        else if (State.IsCarryingFood && simulationEnvironment.AntHill.Coordinates.DistanceFrom(State.Coordinates) <= simulationEnvironment.AntHill.Size)
        {
            return State with
            {
                IsCarryingFood = false,
                PathLength = 1,
                Direction = State.Direction.Opposite(),
            };
        }
        else
        {
            var possibleDirections = ColonyDirection.GeneratePossibleDirections(State.Direction);

            if (randomNumbersGenerator.Next(1000) < 1)
            {
                var newDirection = possibleDirections[randomNumbersGenerator.Next(possibleDirections.Length)];
                return State with
                {
                    Direction = newDirection,
                    PathLength = State.PathLength + 1,
                    Coordinates = MoveCoordinates(simulationEnvironment, State.Coordinates, newDirection)
                };
            }
            else
            {
                var consideringDirections = new List<ColonyDirection>() { possibleDirections[0] };
                var pheromonesStrength = GetPheromonesStrength(simulationEnvironment, State.Coordinates.MoveBy(possibleDirections[0]));
                for (int i = 1; i < possibleDirections.Length; i++)
                {
                    var consideringCoordinates = State.Coordinates.MoveBy(possibleDirections[i]);
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

                var newDirection = consideringDirections[randomNumbersGenerator.Next(consideringDirections.Count)];
                return State with
                {
                    Direction = newDirection,
                    PathLength = State.PathLength + 1,
                    Coordinates = MoveCoordinates(simulationEnvironment, State.Coordinates, newDirection)
                };
            }
        }
    }

    private double GetPheromonesStrength(ColonyEnvironment simulationEnvironment, ColonyCoordinates coordinates)
    {
        var pheromones = simulationEnvironment.GetPheromones(coordinates);
        return State.IsCarryingFood ? pheromones.Home : pheromones.Food;
    }

    private ColonyCoordinates MoveCoordinates(ColonyEnvironment simulationEnvironment, ColonyCoordinates coordinates, ColonyDirection direction)
    {
        var consideringCoordinates = coordinates.MoveBy(direction);
        if (simulationEnvironment.IsInBounds(consideringCoordinates))
        {
            return consideringCoordinates;
        }

        return coordinates;
    }
}

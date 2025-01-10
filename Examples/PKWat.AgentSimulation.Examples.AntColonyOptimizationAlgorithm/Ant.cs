namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm;

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
using System;

public record AntState(ColonyDirection Direction, ColonyCoordinates Coordinates, bool IsCarryingFood, int PathLength)
{
    private const int PheromonesStrengthInitialValue = 1000000;
    public double PheromonesStrength => PheromonesStrengthInitialValue * Math.Pow(0.7, PathLength);
}

public class Ant : SimpleSimulationAgent<ColonyEnvironment>
{
    private const int PheromonesStrengthInitialValue = 1000000;

    public int PathLength { get; private set; } = 0;
    public double PheromonesStrength => PheromonesStrengthInitialValue * Math.Pow(0.7, PathLength);

    //protected override AntState GetInitialState(ColonyEnvironment environment) 
    //    => new AntState(
    //        ColonyDirection.Random(_randomNumbersGenerator),
    //        environment.GetAntHill().Coordinates,
    //        false,
    //        1);

    //protected override AntState GetNextState(ColonyEnvironment environment, IReadOnlySimulationTime simulationTime)
    //{
    //    if (!State.IsCarryingFood && environment.IsNearFood(State.Coordinates))
    //    {
    //        return State with
    //        {
    //            IsCarryingFood = true,
    //            PathLength = 1,
    //            Direction = State.Direction.Opposite()
    //        };
    //    }
        
    //    if (State.IsCarryingFood && environment.IsNearHome(State.Coordinates))
    //    {
    //        return State with
    //        {
    //            IsCarryingFood = false,
    //            PathLength = 1,
    //            Direction = State.Direction.Opposite(),
    //        };
    //    }

    //    var possibleDirections = ColonyDirection.GeneratePossibleDirections(State.Direction);

    //    if (_randomNumbersGenerator.Next(1000) < 1)
    //    {
    //        var newRandomDirection = possibleDirections[_randomNumbersGenerator.Next(possibleDirections.Length)];
    //        return State with
    //        {
    //            Direction = newRandomDirection,
    //            PathLength = State.PathLength + 1,
    //            Coordinates = MoveCoordinates(environment, State.Coordinates, newRandomDirection)
    //        };
    //    }

    //    var consideringDirections = new List<ColonyDirection>() { possibleDirections[0] };
    //    var pheromonesStrength = GetPheromonesStrength(environment, State.Coordinates.MoveBy(possibleDirections[0]));
    //    for (int i = 1; i < possibleDirections.Length; i++)
    //    {
    //        var consideringCoordinates = State.Coordinates.MoveBy(possibleDirections[i]);
    //        var consideringPheromonesStrength = GetPheromonesStrength(environment, consideringCoordinates);
    //        if (consideringPheromonesStrength > pheromonesStrength)
    //        {
    //            consideringDirections.Clear();
    //            consideringDirections.Add(possibleDirections[i]);
    //            pheromonesStrength = consideringPheromonesStrength;
    //        }
    //        else if (consideringPheromonesStrength == pheromonesStrength)
    //        {
    //            consideringDirections.Add(possibleDirections[i]);
    //        }
    //    }

    //    var newDirection = consideringDirections[_randomNumbersGenerator.Next(consideringDirections.Count)];
    //    return State with
    //    {
    //        Direction = newDirection,
    //        PathLength = State.PathLength + 1,
    //        Coordinates = MoveCoordinates(environment, State.Coordinates, newDirection)
    //    };
    //}

    //private double GetPheromonesStrength(ColonyEnvironment simulationEnvironment, ColonyCoordinates coordinates)
    //{
    //    var pheromones = simulationEnvironment.GetPheromones(coordinates);
    //    return State.IsCarryingFood ? pheromones.Home : pheromones.Food;
    //}

    //private ColonyCoordinates MoveCoordinates(ColonyEnvironment simulationEnvironment, ColonyCoordinates coordinates, ColonyDirection direction)
    //{
    //    var consideringCoordinates = coordinates.MoveBy(direction);
    //    if (simulationEnvironment.IsInBounds(consideringCoordinates))
    //    {
    //        return consideringCoordinates;
    //    }

    //    return coordinates;
    //}
}

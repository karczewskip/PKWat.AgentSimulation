namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm;

using PKWat.AgentSimulation.Core;
using System;
using System.Collections;

public class Ant : IAgent<ColonyEnvironment>
{
    private HashSet<ColonyCoordinates> _visitedCoordinates = new HashSet<ColonyCoordinates>();

    private ColonyCoordinates _nextCoordinates;

    public ColonyCoordinates Coordinates { get; private set; }
    public bool IsCarryingFood { get; private set; } = false;

    public Ant(ColonyCoordinates coordinates)
    {
        Coordinates = coordinates;
    }

    public void Decide(ColonyEnvironment simulationEnvironment)
    {
        var possibleMoves = new HashSet<ColonyCoordinates>()
        {
            Coordinates with { X = Coordinates.X - 1 },
            Coordinates with { Y = Coordinates.Y - 1 },
            Coordinates with { X = Coordinates.X + 1 },
            Coordinates with { Y = Coordinates.Y + 1 }
        };

        foreach (var move in possibleMoves)
        {
            if (simulationEnvironment.IsOutOfBounds(move))
            {
                possibleMoves.Remove(move);
            }
        }

        var notVisited = new HashSet<ColonyCoordinates>(possibleMoves);
        foreach (var move in notVisited)
        {
            if (simulationEnvironment.IsOutOfBounds(move))
            {
                notVisited.Remove(move);
            }
        }

        var random = new Random();
        if (notVisited.Any())
        {
            _nextCoordinates = notVisited.ElementAt(random.Next(notVisited.Count));
            return;
        }

        _nextCoordinates = possibleMoves.ElementAt(random.Next(possibleMoves.Count));
    }

    public void Act()
    {
        Coordinates = _nextCoordinates;

        _visitedCoordinates.Add(Coordinates);
    }

}

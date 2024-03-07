namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm;

using PKWat.AgentSimulation.Core;
using System;
using System.Collections;

public class Ant : IAgent<ColonyEnvironment>
{
    private HashSet<ColonyCoordinates> _visitedCoordinates = new HashSet<ColonyCoordinates>();
    private Stack<ColonyCoordinates> _path = new Stack<ColonyCoordinates>();

    private ColonyCoordinates _nextCoordinates;

    public ColonyCoordinates Coordinates { get; private set; }
    public bool IsCarryingFood { get; private set; } = false;
    public bool IsComingHome { get; private set; } = false;

    public int PathLength => _path.Count;

    public void Decide(ColonyEnvironment simulationEnvironment)
    {
        if(PathLength > 500)
        {
            IsComingHome = true;
        }

        if(PathLength == 0) 
        {
            IsComingHome = false;
        }

        if(Coordinates == null)
        {
            _nextCoordinates = simulationEnvironment.AntHill.Coordinates;
        }
        else if(IsComingHome)
        {
            _nextCoordinates = _path.Pop();
        }
        else
        {
            _nextCoordinates = ChooseDirection(simulationEnvironment);
        }
    }

    private ColonyCoordinates ChooseDirection(ColonyEnvironment simulationEnvironment)
    {
        var random = new Random();
        var step = 1 + random.Next(20);

        var possibleMoves = new HashSet<ColonyCoordinates>()
        {
            Coordinates with { X = Coordinates.X - step },
            Coordinates with { Y = Coordinates.Y - step },
            Coordinates with { X = Coordinates.X + step },
            Coordinates with { Y = Coordinates.Y + step }
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
            if (_visitedCoordinates.Contains(move))
            {
                notVisited.Remove(move);
            }
        }
        if (notVisited.Any())
        {
            return notVisited.ElementAt(random.Next(notVisited.Count));
        }

        return possibleMoves.ElementAt(random.Next(possibleMoves.Count));
    }

    public void Act(ColonyEnvironment simulationEnvironment)
    {
        Coordinates = _nextCoordinates;

        if(!IsComingHome)
        {
            _visitedCoordinates.Add(Coordinates);
            _path.Push(Coordinates);
        }
    }

}

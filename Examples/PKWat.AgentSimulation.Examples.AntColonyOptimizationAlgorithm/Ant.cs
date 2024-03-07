namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm;

using PKWat.AgentSimulation.Core;
using System;

public class Ant : IAgent<ColonyEnvironment>
{
    private ColonyCoordinates _nextCoordinates;

    public ColonyCoordinates Coordinates { get; private set; }
    public bool IsCarryingFood { get; private set; } = false;

    public Ant(ColonyCoordinates coordinates)
    {
        Coordinates = coordinates;
    }

    public void Decide(ColonyEnvironment simulationEnvironment)
    {
        if(simulationEnvironment.IsInBounds(_nextCoordinates = Coordinates with { X = Coordinates.X + 1} ))
        {
            return;
        }

        if(simulationEnvironment.IsInBounds(_nextCoordinates = Coordinates with { Y = Coordinates.Y + 1 }))
        {
            return;
        }

        _nextCoordinates = Coordinates;
    }

    public void Act()
    {
        Coordinates = _nextCoordinates;
    }

}

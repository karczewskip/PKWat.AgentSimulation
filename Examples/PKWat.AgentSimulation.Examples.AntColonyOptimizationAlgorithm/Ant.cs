namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm;

using PKWat.AgentSimulation.Core;
using System;

public class Ant : IAgent<ColonyEnvironment>
{
    private double _nextX;
    private double _nextY;

    public double X { get; private set; }
    public double Y { get; private set; }
    public bool IsCarryingFood { get; private set; } = false;

    public Ant(double startX, double startY)
    {
        X = startX;
        Y = startY;
    }

    public void Decide(ColonyEnvironment simulationEnvironment)
    {
        if(simulationEnvironment.IsInBounds(X+1, Y))
        {
            _nextX = X + 1;
            _nextY = Y;
        }
        else if(simulationEnvironment.IsInBounds(X,Y + 1))
        {
            _nextX = X;
            _nextY = Y + 1;
        }
    }

    public void Act()
    {
        X = _nextX;
        Y = _nextY;
    }

}

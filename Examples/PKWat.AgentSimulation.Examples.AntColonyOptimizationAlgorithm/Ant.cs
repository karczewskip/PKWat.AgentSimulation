namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm;

using PKWat.AgentSimulation.Core;
using System;

public class Ant : IAgent
{
    public double X { get; private set; }
    public double Y { get; private set; }
    public bool IsCarryingFood { get; private set; } = false;

    public Ant(double startX, double startY)
    {
        X = startX;
        Y = startY;
    }

    public void Act()
    {
        throw new NotImplementedException();
    }
}

namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
using System;

public class ColonyCoordinates
{
    public int X { get; private set; } = 0;
    public int Y { get; private set; } = 0;

    private ColonyCoordinates() { }

    public static ColonyCoordinates CreateAtOrigin()
    {
        return new ColonyCoordinates();
    }

    public static ColonyCoordinates CreateAt(int x, int y)
    {
        return new ColonyCoordinates { X = x, Y = y };
    }

    public double DistanceFrom(ColonyCoordinates coordinates)
    {
        return Math.Sqrt(Math.Pow(X - coordinates.X, 2) + Math.Pow(Y - coordinates.Y, 2));
    }

    public void MoveBy(ColonyDirection direction)
    {
        X += direction.X;
        Y += direction.Y;
    }

    public void SetCoordinates(int x, int y)
    {
        X = x;
        Y = y;
    }
}

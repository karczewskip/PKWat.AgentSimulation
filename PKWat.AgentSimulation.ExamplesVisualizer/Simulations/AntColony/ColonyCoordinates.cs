namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony;
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

    public double DistanceFrom(double x, double y)
    {
        return Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2));
    }

    public bool IsInRange(ColonyCoordinates coordinates, double size)
    {
        return DistanceFrom(coordinates.X, coordinates.Y) <= size;
    }

    public bool IsInRange(double x, double y, double size)
    {
        return DistanceFrom(x, y) <= size;
    }

    public void MoveBy(ColonyDirection direction, int maxX, int maxY)
    {
        switch (direction)
        {
            case ColonyDirection.Up:
                Y++;
                break;
            case ColonyDirection.UpRight:
                X++;
                Y++;
                break;
            case ColonyDirection.Right:
                X++;
                break;
            case ColonyDirection.DownRight:
                X++;
                Y--;
                break;
            case ColonyDirection.Down:
                Y--;
                break;
            case ColonyDirection.DownLeft:
                X--;
                Y--;
                break;
            case ColonyDirection.Left:
                X--;
                break;
            case ColonyDirection.UpLeft:
                X--;
                Y++;
                break;
        }

        if (X < 0)
        {
            X = 0;
        }
        else if (X >= maxX)
        {
            X = maxX - 1;
        }

        if (Y < 0)
        {
            Y = 0;
        }
        else if (Y >= maxY)
        {
            Y = maxY - 1;
        }
    }

    public void SetCoordinates(int x, int y)
    {
        X = x;
        Y = y;
    }
}

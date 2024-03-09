namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm;

using System;

public record ColonyDirection(int X, int Y)
{
    public static ColonyDirection Random()
    {
        var random = new Random();
        var x = random.Next(-1, 2);
        var y = random.Next(-1, 2);
        return new ColonyDirection(x, y);
    }

    public static ColonyDirection[] GeneratePossibleDirections(ColonyDirection direction)
    {
        if(direction.X != 0 && direction.Y != 0)
        {
            return [
                new ColonyDirection(direction.X, direction.Y),
                new ColonyDirection(direction.X, 0),
                new ColonyDirection(0, direction.Y)
            ];
        }

        if(direction.X == 0)
        {
            return [
                new ColonyDirection(-1, direction.Y),
                new ColonyDirection(0, direction.Y),
                new ColonyDirection(1, direction.Y)
            ];
        }

        return [
            new ColonyDirection(direction.X, -1),
            new ColonyDirection(direction.X, 0),
            new ColonyDirection(direction.X, 1)
            ];

    }

    internal ColonyDirection Opposite()
    {
        return new ColonyDirection(-X, -Y);
    }
}

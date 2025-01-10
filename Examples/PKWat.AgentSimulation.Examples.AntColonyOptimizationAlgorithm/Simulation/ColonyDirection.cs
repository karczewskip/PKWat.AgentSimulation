namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;

using PKWat.AgentSimulation.Core.RandomNumbers;

public class ColonyDirection
{
    public int X { get; private set; }
    public int Y { get; private set; }

    private ColonyDirection()
    {
        
    }

    public static ColonyDirection Random(IRandomNumbersGenerator random)
    {
        var x = random.Next(3) - 1;
        var y = random.Next(3) - 1;
        return new ColonyDirection { X = x, Y = y };
    }

    internal void Opposite()
    {
        X = -X;
        Y = -Y;
    }

    //public static ColonyDirection[] GeneratePossibleDirections(ColonyDirection direction)
    //{
    //    if (direction.X != 0 && direction.Y != 0)
    //    {
    //        return [
    //            new ColonyDirection(direction.X, direction.Y),
    //            new ColonyDirection(direction.X, 0),
    //            new ColonyDirection(0, direction.Y)
    //        ];
    //    }

    //    if (direction.X == 0)
    //    {
    //        return [
    //            new ColonyDirection(-1, direction.Y),
    //            new ColonyDirection(0, direction.Y),
    //            new ColonyDirection(1, direction.Y)
    //        ];
    //    }

    //    return [
    //        new ColonyDirection(direction.X, -1),
    //        new ColonyDirection(direction.X, 0),
    //        new ColonyDirection(direction.X, 1)
    //        ];

    //}
}

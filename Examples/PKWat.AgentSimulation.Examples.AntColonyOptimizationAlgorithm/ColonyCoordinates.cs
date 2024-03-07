namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm
{
    using System;

    public record ColonyCoordinates(int X, int Y)
    {
        public double DistanceFrom(ColonyCoordinates coordinates)
        {
            return Math.Sqrt(Math.Pow(X - coordinates.X, 2) + Math.Pow(Y - coordinates.Y, 2));
        }
    }
}

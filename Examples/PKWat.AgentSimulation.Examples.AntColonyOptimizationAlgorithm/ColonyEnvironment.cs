namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm
{
    using System;

    public class ColonyEnvironment
    {
        public double Width { get; }
        public double Height { get; }

        public AntHill AntHill { get; }

        public ColonyEnvironment(double width, double height, AntHill antHill)
        {
            Width = width;
            Height = height;
            AntHill = antHill;
        }

        public bool IsInBounds(ColonyCoordinates coordinates)
        {
            return coordinates.X >= 0 && coordinates.X < Width && coordinates.Y >= 0 && coordinates.Y < Height;
        }

        public bool IsOutOfBounds(ColonyCoordinates coordinates)
        {
            return !IsInBounds(coordinates);
        }

        public bool IsObstacleAt(double x, double y)
        {
            throw new NotImplementedException();
        }

        public bool IsFoodAt(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void RemoveFood(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void AddFood(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void AddPheromone(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void RemovePheromone(double x, double y)
        {
            throw new NotImplementedException();
        }

        public bool IsPheromoneAt(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void EvaporatePheromones()
        {
            throw new NotImplementedException();
        }
    }
}

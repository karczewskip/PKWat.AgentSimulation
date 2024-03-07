namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ColonyEnvironment
    {
        public double Width { get; }
        public double Height { get; }

        public ColonyEnvironment(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public bool IsInBounds(double x, double y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
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

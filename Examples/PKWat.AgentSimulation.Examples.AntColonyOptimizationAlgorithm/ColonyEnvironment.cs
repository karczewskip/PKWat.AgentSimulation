namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm
{
    using System;

    public class ColonyEnvironment
    {
        private Dictionary<ColonyCoordinates, double> _pheromones = new Dictionary<ColonyCoordinates, double>();

        public int Width { get; }
        public int Height { get; }
        public IReadOnlyDictionary<ColonyCoordinates, double> Pheromones => _pheromones;

        public AntHill AntHill { get; }
        public FoodSource FoodSource { get; }

        public ColonyEnvironment(int width, int height, AntHill antHill, FoodSource foodSource)
        {
            Width = width;
            Height = height;
            AntHill = antHill;
            FoodSource = foodSource;
        }

        public bool IsInBounds(ColonyCoordinates coordinates)
        {
            return coordinates.X >= 0 && coordinates.X < Width && coordinates.Y >= 0 && coordinates.Y < Height;
        }

        public bool IsOutOfBounds(ColonyCoordinates coordinates)
        {
            return !IsInBounds(coordinates);
        }

        public void AddPheromone(ColonyCoordinates coordinates)
        {
            var update = 1;// 1.0 / numberOfMoves;

            if(_pheromones.ContainsKey(coordinates))
            {
                _pheromones[coordinates] += update;
            }
            else
            {
                _pheromones.Add(coordinates, update);
            }
        }

        internal void DecreasePheromones()
        {
            foreach (var pheromone in _pheromones)
            {
                _pheromones[pheromone.Key] -= 0.01;
            }

            foreach (var pheromone in _pheromones)
            {
                if(pheromone.Value < 0.0001)
                {
                    _pheromones.Remove(pheromone.Key);
                }
            }
        }

        public ColonyCoordinates? GetNearestFoodCoordinates(ColonyCoordinates coordinates, int maxDistance)
        {
            if(maxDistance > coordinates.DistanceFrom(FoodSource.Coordinates))
            {
                return FoodSource.Coordinates;
            }

            return null;
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

        public void RemovePheromone(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void EvaporatePheromones()
        {
            throw new NotImplementedException();
        }
    }
}

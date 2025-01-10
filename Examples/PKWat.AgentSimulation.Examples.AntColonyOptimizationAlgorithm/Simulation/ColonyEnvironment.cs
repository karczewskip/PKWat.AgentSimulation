namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;

using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Extensions;
using System;
using System.Collections.Generic;

public class ColonyEnvironment : DefaultSimulationEnvironment
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    private List<FoodSource> FoodSource { get; } = new List<FoodSource>();
    private List<AntHill> AntHills { get; } = new List<AntHill>();
    private Dictionary<ColonyCoordinates, double> FoodPheromones { get; }
    private Dictionary<ColonyCoordinates, double> HomePheromones { get; }

    internal void SetSize(int width, int height)
    {
        Width = width;
        Height = height;
    }

    internal void AddAntHill(AntHill antHill)
    {
        AntHills.Add(antHill);
    }

    internal void AddFoodSource(FoodSource foodSource)
    {
        FoodSource.Add(foodSource);
    }

    internal IReadOnlyList<AntHill> GetAntHills()
    {
        return AntHills;
    }

    internal IReadOnlyList<FoodSource> GetFoodSources()
    {
        return FoodSource;
    }


    //public bool IsNearFood(ColonyCoordinates coordinates)
    //{
    //    return coordinates.DistanceFrom(FoodSource.Coordinates) <= FoodSource.Size;
    //}

    //public bool IsNearHome(ColonyCoordinates coordinates)
    //{
    //    return coordinates.DistanceFrom(AntHill.Coordinates) <= AntHill.Size;
    //}

    public bool IsInBounds(ColonyCoordinates coordinates)
    {
        return coordinates.X >= 0 && coordinates.X < Width && coordinates.Y >= 0 && coordinates.Y < Height;
    }

    public bool IsOutOfBounds(ColonyCoordinates coordinates)
    {
        return !IsInBounds(coordinates);
    }

    internal void DecreasePheromones()
    {
        var minValue = 0.0000001;
        var p = 0.99;

        foreach (var coordinates in FoodPheromones.Keys)
        {
            if (FoodPheromones[coordinates] < minValue)
            {
                FoodPheromones.Remove(coordinates, out _);
            }
            else
            {
                FoodPheromones[coordinates] *= p;
            }
        }

        foreach (var coordinates in HomePheromones.Keys)
        {
            if (HomePheromones[coordinates] < minValue)
            {
                HomePheromones.Remove(coordinates, out _);
            }
            else
            {
                HomePheromones[coordinates] *= p;
            }
        }
    }

    //public ColonyCoordinates? GetNearestFoodCoordinates(ColonyCoordinates coordinates, int maxDistance)
    //{
    //    if (maxDistance > coordinates.DistanceFrom(FoodSource.Coordinates))
    //    {
    //        return FoodSource.Coordinates;
    //    }

    //    return null;
    //}

    public Pheromones GetPheromones(ColonyCoordinates coordinates)
    {
        var homePheromoneValue = HomePheromones.GetValueOrDefault(coordinates, 0);
        var foodPheromoneValue = FoodPheromones.GetValueOrDefault(coordinates, 0);

        return new Pheromones(homePheromoneValue, foodPheromoneValue);
    }

    public void AddFoodPheromones(ColonyCoordinates coordinates, double strength)
    {
        FoodPheromones.AddOrUpdate(coordinates, strength, (c, v) => v + strength);
    }

    public void AddHomePheromones(ColonyCoordinates coordinates, double strength)
    {
        HomePheromones.AddOrUpdate(coordinates, strength, (c, v) => v + strength);
    }

    internal IEnumerable<(ColonyCoordinates Coordinates, Pheromones Pheromones)> GetAllPheromones()
    {
        var allConsideringCoordinates = FoodPheromones.Keys.Union(HomePheromones.Keys);

        return allConsideringCoordinates.Select(c => (c, GetPheromones(c)));
    }
}

public record Pheromones(double Home, double Food);

namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm;

using PKWat.AgentSimulation.Core.Snapshots;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;

public class ColonyEnvironment : ISnapshotCreator
{
    public ConcurrentDictionary<ColonyCoordinates, double> FoodPheromones = new();
    public ConcurrentDictionary<ColonyCoordinates, double> HomePheromones = new();

    public int Width { get; }
    public int Height { get; }

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

    public ColonyCoordinates? GetNearestFoodCoordinates(ColonyCoordinates coordinates, int maxDistance)
    {
        if(maxDistance > coordinates.DistanceFrom(FoodSource.Coordinates))
        {
            return FoodSource.Coordinates;
        }

        return null;
    }

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

    public string CreateSnapshot()
    {
        return JsonSerializer.Serialize(this);
    }
}

public record Pheromones(double Home, double Food);

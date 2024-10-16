namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Extensions;
using System.Collections.Generic;

public record ColonyEnvironmentState(
    int Width, 
    int Height, 
    AntHill AntHill, 
    FoodSource FoodSource, 
    Dictionary<ColonyCoordinates, double> FoodPheromones, 
    Dictionary<ColonyCoordinates, double> HomePheromones);

public class ColonyEnvironment : DefaultSimulationEnvironment<ColonyEnvironmentState>
{
    public override object CreateSnapshot()
    {
        var state = GetState();

        return new
        {
            Width = state.Width,
            Height = state.Height,
            AntHill = state.AntHill,
            FoodSource = state.FoodSource,
            FoodPheromones = state.FoodPheromones.Select(x => new { Coordinates = x.Key, Strength = x.Value }).ToArray(),
            HomePheromones = state.HomePheromones.Select(x => new { Coordinates = x.Key, Strength = x.Value }).ToArray()
        };
    }

    public AntHill GetAntHill()
    {
        return GetState().AntHill;
    }

    public FoodSource GetFoodSource()
    {
        return GetState().FoodSource;
    }

    public IReadOnlyDictionary<ColonyCoordinates, double> GetFoodPheromones()
    {
        return GetState().FoodPheromones;
    }

    public IReadOnlyDictionary<ColonyCoordinates, double> GetHomePheromones()
    {
        return GetState().HomePheromones;
    }

    public bool IsNearFood(ColonyCoordinates coordinates)
    {
        return coordinates.DistanceFrom(GetState().FoodSource.Coordinates) <= GetState().FoodSource.Size;
    }

    public bool IsNearHome(ColonyCoordinates coordinates)
    {
        return coordinates.DistanceFrom(GetState().AntHill.Coordinates) <= GetState().AntHill.Size;
    }

    public bool IsInBounds(ColonyCoordinates coordinates)
    {
        return coordinates.X >= 0 && coordinates.X < GetState().Width && coordinates.Y >= 0 && coordinates.Y < GetState().Height;
    }

    public bool IsOutOfBounds(ColonyCoordinates coordinates)
    {
        return !IsInBounds(coordinates);
    }

    internal void DecreasePheromones()
    {
        var minValue = 0.0000001;
        var p = 0.99;

        foreach (var coordinates in GetState().FoodPheromones.Keys)
        {
            if (GetState().FoodPheromones[coordinates] < minValue)
            {
                GetState().FoodPheromones.Remove(coordinates, out _);
            }
            else
            {
                GetState().FoodPheromones[coordinates] *= p;
            }
        }

        foreach (var coordinates in GetState().HomePheromones.Keys)
        {
            if (GetState().HomePheromones[coordinates] < minValue)
            {
                GetState().HomePheromones.Remove(coordinates, out _);
            }
            else
            {
                GetState().HomePheromones[coordinates] *= p;
            }
        }
    }

    public ColonyCoordinates? GetNearestFoodCoordinates(ColonyCoordinates coordinates, int maxDistance)
    {
        if(maxDistance > coordinates.DistanceFrom(GetState().FoodSource.Coordinates))
        {
            return GetState().FoodSource.Coordinates;
        }

        return null;
    }

    public Pheromones GetPheromones(ColonyCoordinates coordinates)
    {
        var homePheromoneValue = GetState().HomePheromones.GetValueOrDefault(coordinates, 0);
        var foodPheromoneValue = GetState().FoodPheromones.GetValueOrDefault(coordinates, 0);

        return new Pheromones(homePheromoneValue, foodPheromoneValue);
    }

    public void AddFoodPheromones(ColonyCoordinates coordinates, double strength)
    {
        GetState().FoodPheromones.AddOrUpdate(coordinates, strength, (c, v) => v + strength);
    }

    public void AddHomePheromones(ColonyCoordinates coordinates, double strength)
    {
        GetState().HomePheromones.AddOrUpdate(coordinates, strength, (c, v) => v + strength);
    }

    internal IEnumerable<(ColonyCoordinates Coordinates, Pheromones Pheromones)> GetAllPheromones()
    {
        var allConsideringCoordinates = GetState().FoodPheromones.Keys.Union(GetState().HomePheromones.Keys);

        return allConsideringCoordinates.Select(c => (c, GetPheromones(c)));
    }
}

public record Pheromones(double Home, double Food);

namespace PKWat.AgentSimulation.Core;

public interface IRandomNumbersGenerator
{
    int Next(int maxValue);
    double NextDouble();
}

internal class RandomNumbersGenerator : IRandomNumbersGenerator
{
    private static int Counter = 0;
    private static int Seed = 2475;

    private Random _random;

    public RandomNumbersGenerator()
    {
        _random = new Random(Seed + Counter++);
    }

    public int Next(int maxValue)
    {
        return _random.Next(maxValue);
    }

    public double NextDouble()
    {
        return _random.NextDouble();
    }
}


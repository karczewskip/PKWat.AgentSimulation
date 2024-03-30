namespace PKWat.AgentSimulation.Core;

public interface IRandomNumbersGenerator
{
    int Next(int maxValue);
    double NextDouble();
}

internal class RandomNumbersGenerator : IRandomNumbersGenerator
{
    private Random _random;

    public RandomNumbersGenerator(Random random)
    {
        _random = random;
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

internal class RandomNumbersGeneratorFactory
{
    private Random _random;

    public void Initialize(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public IRandomNumbersGenerator Create()
    {
        if(_random == null)
        {
            throw new InvalidOperationException("RandomNumbersGeneratorFactory has not been initialized");
        }

        return new RandomNumbersGenerator(new Random(_random.Next(int.MaxValue)));
    }

}


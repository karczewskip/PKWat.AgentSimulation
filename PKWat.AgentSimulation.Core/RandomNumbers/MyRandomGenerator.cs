namespace PKWat.AgentSimulation.Core.RandomNumbers;

internal class MyRandomGenerator(int? seed = null) : IRandomNumbersGenerator
{
    private long _state = seed ?? new Random().Next();
    private const long _multiplier = 6364136223846793005;
    private const long _increment = 1;
    private const long _modulus = 1L << 32;

    public int Next(int maxValue)
    {
        return (int)(NextDouble() * maxValue);
    }

    public double NextDouble()
    {
        // LCG: X_n+1 = (a * X_n + c) % m
        _state = (_multiplier * _state + _increment) % _modulus;
        return (double)_state / _modulus;
    }

    public double GetNextExponential(double lambda)
    {
        return -Math.Log(1 - NextDouble()) / lambda;
    }
}


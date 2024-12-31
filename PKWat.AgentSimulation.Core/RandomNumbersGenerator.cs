namespace PKWat.AgentSimulation.Core;

public interface IRandomNumbersGenerator
{
    int Next(int maxValue);
    double NextDouble();

    double GetNextExponential(double lambda);
    bool GetNextBool() => NextDouble() < 0.5;
}

internal class MyRandomGenerator(int? seed = null) : IRandomNumbersGenerator
{
    private long _state = seed ?? new Random().Next();
    private const long _multiplier = 6364136223846793005;
    private const long _increment = 1;
    private const long _modulus = (1L << 32);

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

    public double GetNextExponential(double lambda)
    {
        return -Math.Log(1 - _random.NextDouble()) / lambda;
    }

    public double GetNextNormal(double mean, double standardDeviation)
    {
        var u1 = _random.NextDouble();
        var u2 = _random.NextDouble();
        var z0 = Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        return mean + standardDeviation * z0;
    }

    public double GetNextLogNormal(double mean, double standardDeviation)
    {
        var u1 = _random.NextDouble();
        var u2 = _random.NextDouble();
        var z0 = Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        return Math.Exp(mean + standardDeviation * z0);
    }

    public double GetNextTriangular(double a, double b, double c)
    {
        var u = _random.NextDouble();
        if (u < (c - a) / (b - a))
        {
            return a + Math.Sqrt(u * (b - a) * (c - a));
        }
        else
        {
            return b - Math.Sqrt((1 - u) * (b - a) * (b - c));
        }
    }

    public double GetNextUniform(double a, double b)
    {
        return a + (b - a) * _random.NextDouble();
    }

    public double GetNextWeibull(double lambda, double k)
    {
        return lambda * Math.Pow(-Math.Log(1 - _random.NextDouble()), 1 / k);
    }

    public double GetNextPoisson(double lambda)
    {
        var l = Math.Exp(-lambda);
        var k = 0;
        var p = 1.0;
        do
        {
            k++;
            p *= _random.NextDouble();
        } while (p > l);
        return k - 1;
    }

    public double GetNextBinomial(int n, double p)
    {
        var x = 0;
        for (var i = 0; i < n; i++)
        {
            if (_random.NextDouble() < p)
            {
                x++;
            }
        }
        return x;
    }

    public double GetNextGeometric(double p)
    {
        return Math.Floor(Math.Log(1 - _random.NextDouble()) / Math.Log(1 - p));
    }

    public double GetNextHypergeometric(int population, int sample, int success)
    {
        var x = 0;
        var n = population;
        var m = sample;
        var k = success;
        for (var i = 0; i < m; i++)
        {
            if (_random.NextDouble() < (double)k / n)
            {
                k--;
                x++;
            }
            n--;
        }
        return x;
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


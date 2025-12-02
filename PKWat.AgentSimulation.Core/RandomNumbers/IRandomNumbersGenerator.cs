namespace PKWat.AgentSimulation.Core.RandomNumbers;

public interface IRandomNumbersGenerator
{
    int Next(int maxValue);
    double NextDouble();
    double NextDouble(double minValue, double maxValue) => minValue + (NextDouble() * (maxValue - minValue));

    double GetNextExponential(double lambda);
    bool GetNextBool() => NextDouble() < 0.5;
    double GetNextGaussian(double mean, double stddev)
    {
        // Box-Muller transform
        double u1 = 1.0 - NextDouble(); // uniform(0,1] random doubles
        double u2 = 1.0 - NextDouble();

        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stddev * randStdNormal;
    }
}


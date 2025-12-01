namespace PKWat.AgentSimulation.Core.RandomNumbers;

public interface IRandomNumbersGenerator
{
    int Next(int maxValue);
    double NextDouble();
    double NextDouble(double minValue, double maxValue) => minValue + (NextDouble() * (maxValue - minValue));

    double GetNextExponential(double lambda);
    bool GetNextBool() => NextDouble() < 0.5;
}


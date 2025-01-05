namespace PKWat.AgentSimulation.Core.RandomNumbers;

public interface IRandomNumbersGenerator
{
    int Next(int maxValue);
    double NextDouble();

    double GetNextExponential(double lambda);
    bool GetNextBool() => NextDouble() < 0.5;
}


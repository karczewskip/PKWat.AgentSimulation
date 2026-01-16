namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark;

using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Math.Algorithms.TSP;

public class TspTestCaseGenerator
{
    private readonly IRandomNumbersGenerator _randomNumbersGenerator;

    public TspTestCaseGenerator(IRandomNumbersGenerator randomNumbersGenerator)
    {
        _randomNumbersGenerator = randomNumbersGenerator;
    }

    public List<TspPoint> GenerateTestCase(int pointCount, double maxCoordinate)
    {
        var points = new List<TspPoint>();
        
        for (int i = 0; i < pointCount; i++)
        {
            points.Add(TspPoint.Create(
                i, 
                _randomNumbersGenerator.NextDouble() * maxCoordinate, 
                _randomNumbersGenerator.NextDouble() * maxCoordinate));
        }

        return points;
    }
}

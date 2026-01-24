using PKWat.TSPCalculator.Models;

namespace PKWat.TSPCalculator.Utilities;

public static class TspProblemGenerator
{
    public static TspProblem Generate(int numberOfPoints, string name = "Generated Problem", int seed = -1)
    {
        var random = seed >= 0 ? new Random(seed) : new Random();
        
        var problem = new TspProblem
        {
            Name = name,
            Points = new List<Point>()
        };

        for (int i = 0; i < numberOfPoints; i++)
        {
            problem.Points.Add(new Point
            {
                Id = i,
                X = random.NextDouble() * 100, // Points between 0 and 100
                Y = random.NextDouble() * 100
            });
        }

        return problem;
    }
}

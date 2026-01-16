using PKWat.AgentSimulation.SimMath.Algorithms.TSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKWat.AgentSimulation.Math.StatisticsTests.Algorithms.TSP;

internal class AccuracyTests
{
    private readonly CancellationTokenSource _cts = new();

    private readonly ITspAlgorithm _heldKarpAlgorithm = new HeldKarpAlgorithm();
    private readonly ITspAlgorithm _mstPrimAlgorithm = new MstPrimAlgorithm();

    public void RunTests()
    {
        Console.WriteLine("AccuracyTests running...");
        var numberOfTests = 50;
        CalculateStatisticsForRange(numberOfTests, 4, 20);
    }

    private void CalculateStatisticsForRange(int numberOfTests, int numberOfPointsFrom, int numberOfPointsTo)
    {
        for(int i = numberOfPointsFrom; i < numberOfPointsTo; i++)
        {
            CalculateStatisticsFor(numberOfTests, i);
        }
    }

    private void CalculateStatisticsFor(int numberOfTests, int numberOfPoints)
    {
        var differences = CalculateDifferences(numberOfTests, numberOfPoints);
        var averageDifference = differences.Average();
        var maxDifference = differences.Max();
        var minDifference = differences.Min();
        double sumOfSquares = differences.Sum(d => System.Math.Pow(d - averageDifference, 2));
        double standardDeviation = System.Math.Sqrt(sumOfSquares / (differences.Count - 1));
        double standardError = standardDeviation / System.Math.Sqrt(differences.Count);
        double zScore = GetZScore(0.05);
        double lowerBound = averageDifference - (zScore * standardError);
        double upperBound = averageDifference + (zScore * standardError);
        double predictionMargin = zScore * standardDeviation * System.Math.Sqrt(1 + (1 / differences.Count));
        double lower90 = averageDifference - predictionMargin;
        double upper90 = averageDifference + predictionMargin;


        Console.WriteLine($"Statistics for {numberOfPoints} points over {numberOfTests} tests:");
        Console.WriteLine($"Average Difference: {averageDifference:P2}");
        Console.WriteLine($"Max Difference: {maxDifference:P2}");
        Console.WriteLine($"Min Difference: {minDifference:P2}");
        Console.WriteLine($"Standard Deviation: {standardDeviation:P2}");
        Console.WriteLine($"Standard Error: {standardError:P2}");
        Console.WriteLine($"95% Confidence Interval Lower Bound: {lowerBound:P2}");
        Console.WriteLine($"95% Confidence Interval Upper Bound: {upperBound:P2}");
        Console.WriteLine($"Lower: {lower90:P2} and Upper: {upper90:P2}");
        Console.WriteLine();
    }

    /// <summary>
    /// Approximates Z-score for common p-values (one-tailed).
    /// </summary>
    private static double GetZScore(double pValue)
    {
        return pValue switch
        {
            0.10 => 1.282,
            0.05 => 1.645,
            0.01 => 2.326,
            _ => 1.96 // Default to 0.025 (two-tailed 0.05) if not specified
        };
    }

    private List<double> CalculateDifferences(int numberOfTests, int numberOfPoints)
    {
        var differences = new List<double>();
        for (int i = 0; i < numberOfTests; i++)
        {
            var points = GeneratePoints(numberOfPoints);
            var heldKarpResult = _heldKarpAlgorithm.Solve(points, _cts.Token);
            var mstPrimResult = _mstPrimAlgorithm.Solve(points, _cts.Token);
            differences.Add(CalculateDifference(heldKarpResult, mstPrimResult));
        }
        return differences;
    }

    private double CalculateDifference(TspSolution heldKarpResult, TspSolution mstPrimResult)
    {
        return (mstPrimResult.TotalDistance - heldKarpResult.TotalDistance) / heldKarpResult.TotalDistance;
    }

    private List<TspPoint> GeneratePoints(int numberOfPoints)
    {
        var points = new List<TspPoint>();
        var random = new Random();
        for (int i = 0; i < numberOfPoints; i++)
        {
            double x = random.NextDouble() * 100;
            double y = random.NextDouble() * 100;
            points.Add(TspPoint.Create(i, x, y));
        }
        return points;
    }
}

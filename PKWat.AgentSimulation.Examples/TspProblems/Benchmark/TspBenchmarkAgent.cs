namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark;

using PKWat.AgentSimulation.Core.Agent;
using System.Diagnostics;

public enum TspAlgorithmType
{
    BruteForce,
    HeldKarp,
    MstPrim
}

public class TspBenchmarkAgent : SimpleSimulationAgent
{
    public TspAlgorithmType AlgorithmType { get; set; }
    public TspSolution? BestSolution { get; private set; }
    public bool IsComplete { get; private set; }
    public bool HasExceededTimeLimit { get; private set; }
    public int CurrentPointCount { get; private set; }
    public int CurrentExampleIndex { get; private set; }
    public TimeSpan TimeLimit { get; private set; }
    public Stopwatch Stopwatch { get; private set; }
    public List<BenchmarkResult> Results { get; private set; }
    
    public TspBenchmarkAgent()
    {
        BestSolution = TspSolution.Empty();
        IsComplete = false;
        HasExceededTimeLimit = false;
        CurrentPointCount = 0;
        CurrentExampleIndex = 0;
        Stopwatch = new Stopwatch();
        Results = new List<BenchmarkResult>();
        TimeLimit = TimeSpan.FromSeconds(60);
    }

    public void SetTimeLimit(TimeSpan timeLimit)
    {
        TimeLimit = timeLimit;
    }

    public void StartNewRound(int pointCount, int exampleIndex)
    {
        CurrentPointCount = pointCount;
        CurrentExampleIndex = exampleIndex;
        BestSolution = TspSolution.Empty();
        IsComplete = false;
        Stopwatch.Restart();
    }

    public void SetBestSolution(TspSolution solution)
    {
        BestSolution = solution;
    }

    public void MarkComplete()
    {
        IsComplete = true;
        Stopwatch.Stop();
        
        if (Stopwatch.Elapsed <= TimeLimit)
        {
            Results.Add(new BenchmarkResult
            {
                PointCount = CurrentPointCount,
                ExampleIndex = CurrentExampleIndex,
                Distance = BestSolution!.TotalDistance,
                ExecutionTime = Stopwatch.Elapsed,
                ExceededTimeLimit = false
            });
        }
        else
        {
            HasExceededTimeLimit = true;
            Results.Add(new BenchmarkResult
            {
                PointCount = CurrentPointCount,
                ExampleIndex = CurrentExampleIndex,
                Distance = double.MaxValue,
                ExecutionTime = Stopwatch.Elapsed,
                ExceededTimeLimit = true
            });
        }
    }

    public bool CheckTimeLimit()
    {
        if (Stopwatch.Elapsed > TimeLimit)
        {
            HasExceededTimeLimit = true;
            MarkComplete();
            return true;
        }
        return false;
    }
}

public class BenchmarkResult
{
    public int PointCount { get; init; }
    public int ExampleIndex { get; init; }
    public double Distance { get; init; }
    public TimeSpan ExecutionTime { get; init; }
    public bool ExceededTimeLimit { get; init; }
}

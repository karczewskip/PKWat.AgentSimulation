using System.Collections.Concurrent;

namespace PKWat.AgentSimulation.Core.PerformanceInfo;

public interface ISimulationCyclePerformanceInfo
{
    IDisposable AddStep(string name);
    string GetPerformanceInfo();
}

internal class SimulationPerformanceInfo : ISimulationCyclePerformanceInfo
{
    private List<SimulationPerformanceInfoStep> _cycles = new();
    private ConcurrentBag<SimulationPerformanceInfoStep> _currentCycleSteps = new();

    private SimulationPerformanceInfoStep? CurrentCycle => _cycles.LastOrDefault();

    public void Clear()
    {
        _cycles.Clear();
        _currentCycleSteps.Clear();
    }

    public void StartNewCycle()
    {
        CurrentCycle?.Stop();
        _currentCycleSteps.Clear();

        var newCycle = SimulationPerformanceInfoStep.Start(GetCycleName(_cycles.Count + 1));
        _cycles.Add(newCycle);
    }

    public IDisposable AddStep(string name)
    {
        var newStep = SimulationPerformanceInfoStep.Start(name);
        _currentCycleSteps.Add(newStep);

        return new DisposeAfterFinish(() => newStep.Stop());
    }

    public string GetPerformanceInfo()
    {
        var fpsLine = $"FPS: {CalculateFpsBasedOnLastCycles(10):F2}";
        var existingSteps = _currentCycleSteps.ToArray();
        var calculatedSteps = existingSteps.GroupBy(x => x.Name).Select(x => (Name: x.Key, Avg: x.Select(y => y.Ellapsed.Milliseconds).Average()));
        var currentStepsInfo = string.Join("\n", calculatedSteps.Select(x => $"{x.Name}: {x.Avg} ms"));

        return string.Join("\n", fpsLine, currentStepsInfo);
    }

    private double CalculateFpsBasedOnLastCycles(int numberOfCycles)
    {
        var usingCycles = _cycles.SkipLast(1).TakeLast(numberOfCycles).ToArray();

        if(usingCycles.Length == 0)
        {
            return 0;
        }

        var meanTime = usingCycles.Select(x => x.Ellapsed.TotalMilliseconds).Average();
        var fps = 1000 / meanTime;

        return fps;
    }

    private static string GetCycleName(int cycleNumber)
    {
        return $"Cycle {cycleNumber}";
    }

    private class DisposeAfterFinish : IDisposable
    {
        private readonly Action _onDispose;
        public DisposeAfterFinish(Action onDispose)
        {
            _onDispose = onDispose;
        }
        public void Dispose()
        {
            _onDispose();
        }
    }
}

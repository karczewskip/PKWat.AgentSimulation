using System.Collections.Concurrent;

namespace PKWat.AgentSimulation.Core.PerformanceInfo;

public interface ISimulationCyclePerformanceInfo
{
    IDisposable AddStep(string name);
    string GetPerformanceInfo();
    void Subscribe(Action<string> notify);
}

internal class SimulationPerformanceInfo : ISimulationCyclePerformanceInfo
{
    private const int MaxCycleNumbers = 1000;

    private List<SimulationPerformanceInfoStep> _cycles = new();
    private ConcurrentBag<SimulationPerformanceInfoStep> _currentCycleSteps = new();
    private List<Action<string>> _subscribers = new();

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
        if(_cycles.Count > MaxCycleNumbers)
        {
            _cycles.RemoveAt(0);
        }
    }

    public IDisposable AddStep(string name)
    {
        var newStep = SimulationPerformanceInfoStep.Start(name);
        _currentCycleSteps.Add(newStep);

        return new DisposeAfterFinish(() => newStep.Stop());
    }

    public string GetPerformanceInfo()
    {
        var fpsLine = $"FPS: {CalculateFpsBasedOnLastCycles():F2}";
        var existingSteps = _currentCycleSteps.GroupBy(x => x.Name).ToArray();
        var calculatedSteps = existingSteps.Select(x => new[] 
        { 
            (Name: $"{x.Key}(avg)", Value: x.Select(y => y.Ellapsed.TotalNanoseconds).Average()),
            //(Name: $"{x.Key}(total)", Value: x.Select(y => y.Ellapsed.TotalNanoseconds).Sum()),
        }).SelectMany(x => x);
        var currentStepsInfo = string.Join("\n", calculatedSteps.Select(x => $"{x.Name}: {x.Value:F2} ns"));

        return string.Join("\n", fpsLine, currentStepsInfo);
    }

    private double CalculateFpsBasedOnLastCycles()
    {
        var usingCycles = _cycles.SkipLast(1).ToArray();

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

    public void Subscribe(Action<string> notify)
    {
        _subscribers.Add(notify);
    }

    public void NotifySubscribers()
    {
        var performanceInfo = GetPerformanceInfo();
        foreach (var subscriber in _subscribers)
        {
            subscriber(performanceInfo);
        }
    }

    private class DisposeAfterFinish(Action onDispose) : IDisposable
    {
        public void Dispose()
        {
            onDispose();
        }
    }
}

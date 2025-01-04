namespace PKWat.AgentSimulation.Core.PerformanceInfo;

public interface IReadOnlySimulationPerformanceInfo
{
    string GetPerformanceInfo();
}

public class SimulationPerformanceInfo : IReadOnlySimulationPerformanceInfo
{
    private List<SimulationPerformanceInfoStep> _cycles = new List<SimulationPerformanceInfoStep>();
    private List<SimulationPerformanceInfoStep> _currentCycleSteps = new List<SimulationPerformanceInfoStep>();

    private SimulationPerformanceInfoStep? CurrentCycle => _cycles.LastOrDefault();

    private SimulationPerformanceInfo()
    {
    }

    public static SimulationPerformanceInfo CreateWithFirstCycle()
    {
        var performanceInfo = new SimulationPerformanceInfo();

        performanceInfo.StartNewCycle();

        return performanceInfo;
    }

    public void StartNewCycle()
    {
        CurrentCycle?.Stop();
        _currentCycleSteps.Clear();

        var newCycle = SimulationPerformanceInfoStep.Start(GetCycleName(_cycles.Count + 1));
        _cycles.Add(newCycle);
    }

    public SimulationPerformanceInfoStep AddStep(string name)
    {
        var newStep = SimulationPerformanceInfoStep.Start(name);
        _currentCycleSteps.Add(newStep);

        return newStep;
    }

    public string GetPerformanceInfo()
    {
        var fpsLine = $"FPS: {CalculateFpsBasedOnLastCycles(10)}";
        var currentStepsInfo = string.Join("\n", _currentCycleSteps.Select(x => $"{x.Name}: {x.Ellapsed.Milliseconds} ms"));

        return string.Join("\n", fpsLine, currentStepsInfo);
    }

    private double CalculateFpsBasedOnLastCycles(int numberOfCycles)
    {
        var usingCycles = _cycles.SkipLast(1).TakeLast(numberOfCycles);

        var meanTime = usingCycles.Select(x => x.Ellapsed.TotalMilliseconds).Average();
        var fps = 1000 / meanTime;

        return fps;
    }

    private static string GetCycleName(int cycleNumber)
    {
        return $"Cycle {cycleNumber}";
    }
}

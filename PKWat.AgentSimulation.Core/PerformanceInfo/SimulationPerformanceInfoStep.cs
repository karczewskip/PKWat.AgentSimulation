using System.Diagnostics;

namespace PKWat.AgentSimulation.Core.PerformanceInfo;

public class SimulationPerformanceInfoStep
{
    private Stopwatch _stopwatch = new Stopwatch();

    public string Name { get; private set; }

    public bool IsFinished => _stopwatch.IsRunning == false;
    public TimeSpan Ellapsed => _stopwatch.Elapsed;

    private SimulationPerformanceInfoStep(string name, Stopwatch stopwatch)
    {
        Name = name;
        _stopwatch = stopwatch;
    }

    public static SimulationPerformanceInfoStep Start(string name)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        return new SimulationPerformanceInfoStep(name, stopwatch);
    }

    public void Stop()
    {
        _stopwatch.Stop();
    }
}

namespace PKWat.AgentSimulation.Core;

public class SimulationPerformanceInfo
{
    private readonly List<SimulationPerformanceInfoStep> _steps = new List<SimulationPerformanceInfoStep>();

    public void AddStep(SimulationPerformanceInfoStep step)
    {
        _steps.Add(step);
    }
}

public class SimulationPerformanceInfoStep
{
    public string Name { get; private set; }
    public TimeSpan Start { get; private set; }
    public TimeSpan? End { get; private set; }

    private SimulationPerformanceInfoStep(string name, TimeSpan start, TimeSpan? end)
    {
        Name = name;
        Start = start;
        End = end;
    }

    public static SimulationPerformanceInfoStep WithStart(string name, TimeSpan start)
    {
        return new SimulationPerformanceInfoStep(name, start, null);
    }

    public SimulationPerformanceInfoStep SetEnd(TimeSpan end)
    {
        return new SimulationPerformanceInfoStep(Name, Start, end);
    }
}

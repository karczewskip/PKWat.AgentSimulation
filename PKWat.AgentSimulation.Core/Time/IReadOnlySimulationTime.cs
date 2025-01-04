namespace PKWat.AgentSimulation.Core.Time;

public interface IReadOnlySimulationTime
{
    TimeSpan Time { get; }
    TimeSpan Step { get; }
    long StepNo { get; }
}

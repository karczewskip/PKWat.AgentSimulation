namespace PKWat.AgentSimulation.Core.Time;

public record SimulationTime(TimeSpan Time, TimeSpan Step, long StepNo = 0) : IReadOnlySimulationTime
{
    public SimulationTime AddStep() => this with { Time = Time + Step, StepNo = StepNo + 1 };
}

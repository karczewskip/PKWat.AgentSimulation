namespace PKWat.AgentSimulation.Core.Time;

public record SimulationTime(TimeSpan Time, TimeSpan Step, long StepNo = 0) 
    : IReadOnlySimulationTime
{
    public SimulationTime AddStep(TimeSpan step) => new(Time + step, step, StepNo + 1);

    public SimulationTime SetTime(TimeSpan newTime) => new(newTime, newTime - Time, StepNo + 1);

    public static SimulationTime CreateZero() => new(TimeSpan.Zero, TimeSpan.Zero, 0);
}

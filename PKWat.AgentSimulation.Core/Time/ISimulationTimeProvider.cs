namespace PKWat.AgentSimulation.Core.Time;

public interface ISimulationTimeProvider
{
    IReadOnlySimulationTime Time { get; }
}

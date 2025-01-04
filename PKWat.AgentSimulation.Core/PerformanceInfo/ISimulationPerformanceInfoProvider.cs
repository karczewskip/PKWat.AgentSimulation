namespace PKWat.AgentSimulation.Core.PerformanceInfo;

public interface ISimulationPerformanceInfoProvider
{
    IReadOnlySimulationPerformanceInfo PerformanceInfo { get; }
}

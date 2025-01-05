namespace PKWat.AgentSimulation.Core.PerformanceInfo;

public interface ISimulationPerformanceInfoProvider
{
    ISimulationCyclePerformanceInfo PerformanceInfo { get; }
}

namespace PKWat.AgentSimulation.Core.Environment;

using PKWat.AgentSimulation.Core.Crash;
using PKWat.AgentSimulation.Core.Snapshots;

public interface ISimulationEnvironment
{
    SimulationCrashResult CheckCrashConditions();
}

public interface ISimulationEnvironment<SIMULATION_STATE> : ISimulationEnvironment, ISnapshotCreator
{
    void LoadState(SIMULATION_STATE state);
}
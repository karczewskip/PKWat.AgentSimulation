namespace PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.Snapshots;
using PKWat.AgentSimulation.Core.Time;


public interface ISimulationAgent
{

}

public interface ISimulationAgent<ENVIRONMENT> : ISimulationAgent, IRecognizableAgent, ISnapshotCreator where ENVIRONMENT : ISimulationEnvironment
{
    void Initialize(ENVIRONMENT environment);
    void Act(ENVIRONMENT environment, IReadOnlySimulationTime simulationTime);
    bool ShouldBeRemovedFromSimulation(IReadOnlySimulationTime simulationTime);
}

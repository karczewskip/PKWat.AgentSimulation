namespace PKWat.AgentSimulation.Core.Agent;

using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.Time;

public abstract class SimpleSimulationAgent<ENVIRONMENT> : ISimulationAgent<ENVIRONMENT> where ENVIRONMENT : ISimulationEnvironment
{
    public AgentId Id { get; } = AgentId.GenerateNew();

    public virtual void Initialize(ENVIRONMENT environment)
    {

    }

    public virtual void Act(ENVIRONMENT environment, IReadOnlySimulationTime simulationTime)
    {

    }

    public object CreateSnapshot()
    {
        return new { Id };
    }

    public bool Equals(IRecognizableAgent? other)
    {
        return other is IRecognizableAgent agent && agent.Id == Id;
    }


    public virtual bool ShouldBeRemovedFromSimulation(IReadOnlySimulationTime simulationTime)
    {
        return false;
    }
}

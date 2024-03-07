namespace PKWat.AgentSimulation.Core;

using System.Collections.Generic;

public interface ISimulationContext
{
    IEnumerable<T> GetAgents<T>() where T : IAgent;
}

internal class SimulationContext: ISimulationContext
{
    public SimulationContext(List<IAgent> agents, TimeSpan waitingTimeBetweenSteps)
    {
        Agents = agents;
        WaitingTimeBetweenSteps = waitingTimeBetweenSteps;
    }

    public IReadOnlyList<IAgent> Agents { get; }
    public TimeSpan WaitingTimeBetweenSteps { get; }

    public IEnumerable<T> GetAgents<T>() where T : IAgent 
        => Agents.OfType<T>();
}

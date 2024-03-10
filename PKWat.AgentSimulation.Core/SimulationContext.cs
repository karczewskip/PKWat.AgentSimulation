namespace PKWat.AgentSimulation.Core;

using System.Collections.Generic;

public interface ISimulationContext<U>
{
    U SimulationEnvironment { get; }
    IEnumerable<T> GetAgents<T>() where T : IAgent<U>;
}

internal class SimulationContext<U>: ISimulationContext<U>
{
    public SimulationContext(U simulationEnvironment, List<IAgent<U>> agents, TimeSpan waitingTimeBetweenSteps)
    {
        SimulationEnvironment = simulationEnvironment;
        Agents = agents;
        WaitingTimeBetweenSteps = waitingTimeBetweenSteps;
    }

    public U SimulationEnvironment { get; }
    public IReadOnlyList<IAgent<U>> Agents { get; }
    public TimeSpan WaitingTimeBetweenSteps { get; }

    public IEnumerable<T> GetAgents<T>() where T : IAgent<U> 
        => Agents.OfType<T>();
}

public interface IRandomNumbersGenerator
{
    
}

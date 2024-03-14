namespace PKWat.AgentSimulation.Core;

using System.Collections.Generic;

public interface ISimulationContext<U>
{
    U SimulationEnvironment { get; }
    TimeSpan SimulationStep { get; }
    TimeSpan SimulationTime { get; }

    IEnumerable<T> GetAgents<T>() where T : IAgent<U>;
}

internal class SimulationContext<U>: ISimulationContext<U>
{
    public SimulationContext(U simulationEnvironment, List<IAgent<U>> agents, TimeSpan simulationStep, TimeSpan waitingTimeBetweenSteps)
    {
        SimulationEnvironment = simulationEnvironment;
        Agents = agents;
        SimulationStep = simulationStep;
        SimulationTime = TimeSpan.Zero;
        WaitingTimeBetweenSteps = waitingTimeBetweenSteps;
    }

    public U SimulationEnvironment { get; }
    public TimeSpan SimulationStep { get; }
    public TimeSpan SimulationTime { get; private set; }

    public IReadOnlyList<IAgent<U>> Agents { get; }
    public TimeSpan WaitingTimeBetweenSteps { get; }

    public IEnumerable<T> GetAgents<T>() where T : IAgent<U> 
        => Agents.OfType<T>();

    internal void UpdateSimulationTime()
    {
        SimulationTime += SimulationStep;
    }
}
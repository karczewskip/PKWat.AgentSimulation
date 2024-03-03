namespace PKWat.AgentSimulation.Core;

using System.Collections.Generic;

internal class SimulationContext
{
    public SimulationContext(List<IAgent> agents, List<Func<Task>> callbacks, TimeSpan waitingTimeBetweenSteps)
    {
        Agents = agents;
        Callbacks = callbacks;
        WaitingTimeBetweenSteps = waitingTimeBetweenSteps;
    }

    public IReadOnlyList<IAgent> Agents { get; }
    public IReadOnlyList<Func<Task>> Callbacks { get; }
    public TimeSpan WaitingTimeBetweenSteps { get; }
}

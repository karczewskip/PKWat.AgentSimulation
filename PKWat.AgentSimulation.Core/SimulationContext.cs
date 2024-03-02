namespace PKWat.AgentSimulation.Core;

using System.Collections.Generic;

internal class SimulationContext
{
    public SimulationContext(List<IAgent> agents, List<Func<Task>> callbacks)
    {
        Agents = agents;
        Callbacks = callbacks;
        WaitingTimeBetweenSteps = TimeSpan.FromMilliseconds(2);
    }

    public IReadOnlyList<IAgent> Agents { get; }
    public IReadOnlyList<Func<Task>> Callbacks { get; }
    public TimeSpan WaitingTimeBetweenSteps { get; }
}

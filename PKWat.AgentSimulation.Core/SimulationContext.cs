namespace PKWat.AgentSimulation.Core;

using System.Collections.Generic;

internal class SimulationContext
{
    public IReadOnlyList<IAgent> Agents;

    public SimulationContext(List<IAgent> agents)
    {
        Agents = agents;
    }
}

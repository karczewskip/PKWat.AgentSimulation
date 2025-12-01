using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Environment;
using System.Collections.Concurrent;

namespace PKWat.AgentSimulation.Genetic.SeparateAgents.Logic;

internal class CalculationsBlackboard : DefaultSimulationEnvironment
{
    public ConcurrentDictionary<AgentId, ErrorResult> AgentErrors { get; } = new();
    public ExpectedValues ExpectedValues { get; private set; }

    public void SetExpectedValues(ExpectedValues expectedValues)
    {
        ExpectedValues = expectedValues;
    }
}

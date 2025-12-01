using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

namespace PKWat.AgentSimulation.Genetic.SeparateAgents.Logic.Stages;

internal class InitializeBlackboard : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<CalculationsBlackboard>();
        environment.SetExpectedValues(
            ExpectedValues.Build(
                Enumerable.Range(0, 50_000).Select(x => (double)x), 
                x => x * x + 2 * x + 3));
    }
}

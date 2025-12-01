using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

namespace PKWat.AgentSimulation.Genetic.SeparateAgents.Logic.Stages;

internal class CalculateForAllAgents : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var blackboard = context.GetSimulationEnvironment<CalculationsBlackboard>();
        var agents = context.GetAgents<PolynomialCheckAgent>();

        blackboard.AgentErrors.Clear();
        // Calculate in separate threads
        Parallel.ForEach(agents, (agent, ct) =>
        {
            var errorsForAgent = agent.CalculateError(blackboard.ExpectedValues);
            blackboard.AgentErrors[agent.Id] = errorsForAgent;
        });
    }
}

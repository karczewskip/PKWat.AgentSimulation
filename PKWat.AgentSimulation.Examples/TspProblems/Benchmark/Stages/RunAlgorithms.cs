namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class RunAlgorithms : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspBenchmarkEnvironment>();
        var agents = context.GetAgents<TspBenchmarkAgent>()
            .Where(a => !a.IsComplete && !a.HasExceededTimeLimit)
            .ToList();

        if (agents.Count == 0 || environment.CurrentPoints.Count == 0)
            return;

        foreach (var agent in agents.OrderBy(a => a.AlgorithmType))
        {
            agent.ExecuteTestCase(
                environment.CurrentPoints,
                environment.CurrentPointCount,
                environment.CurrentExampleIndex);
        }

        await Task.CompletedTask;
    }
}

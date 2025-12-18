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
            // Start timing for this test case
            agent.StartNewRound(environment.CurrentPointCount, environment.CurrentExampleIndex);

            // Check time limit before starting
            if (agent.CheckTimeLimit())
                continue;

            // Run the algorithm with cancellation support
            var solution = agent.Algorithm.Solve(environment.CurrentPoints, agent.GetCancellationToken());

            // If solution is null, it means the algorithm was cancelled
            if (solution == null)
            {
                agent.CheckTimeLimit(); // This will mark as timeout
                continue;
            }

            // Set the solution
            agent.SetBestSolution(solution);
            
            // Mark as complete
            agent.MarkComplete();
        }

        await Task.CompletedTask;
    }
}

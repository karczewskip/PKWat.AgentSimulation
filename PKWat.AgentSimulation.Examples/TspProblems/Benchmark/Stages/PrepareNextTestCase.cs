namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class PrepareNextTestCase : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspBenchmarkEnvironment>();
        var agents = context.GetAgents<TspBenchmarkAgent>().ToList();

        // Check if any agent is still active
        var activeAgents = agents.Where(a => !a.HasExceededTimeLimit).ToList();
        
        if (activeAgents.Count == 0)
            return;

        // Check if all active agents have completed the current test case
        bool allComplete = activeAgents.All(a => a.IsComplete);
        
        if (!allComplete)
            return;

        // Reset completion flags for the next test case
        foreach (var agent in activeAgents)
        {
            agent.ResetCompletion();
        }

        // Move to next test case
        environment.MoveToNextExample();
        environment.LoadCurrentTestCase();

        await Task.CompletedTask;
    }
}

namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class PrepareNextTestCase : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspBenchmarkEnvironment>();
        var agents = context.GetAgents<TspBenchmarkAgent>().Where(a => !a.HasExceededTimeLimit).ToList();

        if (agents.Count == 0)
            return;

        bool allComplete = agents.All(a => a.IsComplete);
        
        if (!allComplete)
            return;

        environment.MoveToNextExample();
        environment.LoadCurrentTestCase();

        foreach (var agent in agents)
        {
            agent.StartNewRound(environment.CurrentPointCount, environment.CurrentExampleIndex);
        }

        await Task.CompletedTask;
    }
}

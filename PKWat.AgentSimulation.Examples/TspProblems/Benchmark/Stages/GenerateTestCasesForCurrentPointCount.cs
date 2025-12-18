namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class GenerateTestCasesForCurrentPointCount : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspBenchmarkEnvironment>();

        environment.GenerateTestCasesForPointCount();

        await Task.CompletedTask;
    }
}

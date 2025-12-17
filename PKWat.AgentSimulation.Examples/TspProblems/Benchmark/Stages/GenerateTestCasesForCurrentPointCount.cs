namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class GenerateTestCasesForCurrentPointCount : ISimulationStage
{
    private readonly Random _random = new Random(42);

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspBenchmarkEnvironment>();

        // Only generate if we haven't generated for this point count yet
        if (environment.CurrentExampleIndex == 0)
        {
            environment.GenerateTestCasesForPointCount(environment.CurrentPointCount, _random);
        }

        // Always load the current test case
        environment.LoadCurrentTestCase();

        await Task.CompletedTask;
    }
}

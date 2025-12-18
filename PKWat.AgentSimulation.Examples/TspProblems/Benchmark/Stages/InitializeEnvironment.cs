namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class InitializeEnvironment : ISimulationStage
{
    private int _startingPointCount = 3;

    public void SetStartingPointCount(int startingPointCount)
    {
        _startingPointCount = startingPointCount;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspBenchmarkEnvironment>();
        environment.SetStartingPointCount(_startingPointCount);

        await Task.CompletedTask;
    }
}

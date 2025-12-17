namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class InitializeBenchmark : ISimulationStage
{
    private int _startingPointCount = 4;
    private TimeSpan _timeLimit = TimeSpan.FromSeconds(60);

    public void SetStartingPointCount(int startingPointCount)
    {
        _startingPointCount = startingPointCount;
    }

    public void SetTimeLimit(TimeSpan timeLimit)
    {
        _timeLimit = timeLimit;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspBenchmarkEnvironment>();
        var agents = context.GetAgents<TspBenchmarkAgent>().ToList();

        environment.SetStartingPointCount(_startingPointCount);

        foreach (var agent in agents)
        {
            agent.SetTimeLimit(_timeLimit);
        }

        await Task.CompletedTask;
    }
}

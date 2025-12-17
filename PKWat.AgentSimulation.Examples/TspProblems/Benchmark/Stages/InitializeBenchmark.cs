namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class InitializeBenchmark : ISimulationStage
{
    private int _maxPointCount = 15;
    private int _startingPointCount = 3;
    private TimeSpan _timeLimit = TimeSpan.FromSeconds(60);

    public void SetMaxPointCount(int maxPointCount)
    {
        _maxPointCount = maxPointCount;
    }

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
        
        var random = new Random(42);
        environment.GenerateTestCases(_maxPointCount, random);

        foreach (var agent in agents)
        {
            agent.SetTimeLimit(_timeLimit);
        }

        environment.LoadCurrentTestCase();

        await Task.CompletedTask;
    }
}

namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Core.Crash;
using PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Stages;

public class TspBenchmarkSimulationBuilder
{
    private readonly ISimulationBuilder _simulationBuilder;

    public TspBenchmarkSimulationBuilder(ISimulationBuilder simulationBuilder)
    {
        _simulationBuilder = simulationBuilder;
    }

    public ISimulation Build(TimeSpan? timeLimit = null)
    {
        var actualTimeLimit = timeLimit ?? TimeSpan.FromSeconds(3);

        var simulation = _simulationBuilder
            .CreateNewSimulation<TspBenchmarkEnvironment>()
            .AddAgent<TspBenchmarkAgent>(a => a.AlgorithmType = TspAlgorithmType.BruteForce)
            .AddAgent<TspBenchmarkAgent>(a => a.AlgorithmType = TspAlgorithmType.HeldKarp)
            .AddAgent<TspBenchmarkAgent>(a => a.AlgorithmType = TspAlgorithmType.MstPrim)
            .AddInitializationStage<InitializeBenchmark>(s =>
            {
                s.SetTimeLimit(actualTimeLimit);
            })
            .AddStage<RunBruteForce>()
            .AddStage<RunHeldKarp>()
            .AddStage<RunMstPrim>()
            .AddStage<PrepareNextTestCase>()
            .AddCrashCondition(StopWhenOneOrZeroAgentsRemaining)
            .SetRandomSeed(42)
            .Build();

        return simulation;
    }

    private SimulationCrashResult StopWhenOneOrZeroAgentsRemaining(ISimulationContext context)
    {
        var agents = context.GetAgents<TspBenchmarkAgent>().ToList();
        var activeAgents = agents.Where(a => !a.HasExceededTimeLimit).ToList();

        if (activeAgents.Count <= 1)
        {
            return SimulationCrashResult.Crash($"Benchmark complete: {activeAgents.Count} agent(s) remaining within time limit");
        }

        return SimulationCrashResult.NoCrash;
    }
}

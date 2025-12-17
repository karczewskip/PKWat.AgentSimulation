namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.TspProblems.Benchmark;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Core.Crash;
using PKWat.AgentSimulation.Examples.TspProblems.Benchmark;
using PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Stages;
using System;
using System.Windows.Media.Imaging;

public class TspBenchmarkSimulationBuilder(
    ISimulationBuilder simulationBuilder,
    TspBenchmarkDrawer drawer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        drawer.InitializeIfNeeded(1000, 800);

        var simulation = simulationBuilder
            .CreateNewSimulation<TspBenchmarkEnvironment>()
            .AddAgent<TspBenchmarkAgent>(a => a.AlgorithmType = TspAlgorithmType.BruteForce)
            .AddAgent<TspBenchmarkAgent>(a => a.AlgorithmType = TspAlgorithmType.HeldKarp)
            .AddAgent<TspBenchmarkAgent>(a => a.AlgorithmType = TspAlgorithmType.MstPrim)
            .AddInitializationStage<InitializeBenchmark>(s =>
            {
                s.SetMaxPointCount(15);
                s.SetStartingPointCount(3);
                s.SetTimeLimit(TimeSpan.FromSeconds(10));
            })
            .AddStage<RunBruteForce>()
            .AddStage<RunHeldKarp>()
            .AddStage<RunMstPrim>()
            .AddStage<PrepareNextTestCase>()
            .AddCallback(c => drawing(drawer.Draw(c)))
            .AddCrashCondition(c =>
            {
                var agents = c.GetAgents<TspBenchmarkAgent>().ToList();
                var activeAgents = agents.Where(a => !a.HasExceededTimeLimit).ToList();

                if (activeAgents.Count <= 1)
                {
                    return SimulationCrashResult.Crash($"Benchmark complete: {activeAgents.Count} agent(s) remaining within time limit");
                }

                return SimulationCrashResult.NoCrash;
            })
            .SetRandomSeed(42)
            .Build();

        return simulation;
    }
}

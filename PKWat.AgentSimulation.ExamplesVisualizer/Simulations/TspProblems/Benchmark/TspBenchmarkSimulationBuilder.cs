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

        var timeLimit = TimeSpan.FromSeconds(0.2);

        var simulation = simulationBuilder
            .CreateNewSimulation<TspBenchmarkEnvironment>()
            .AddAgent<TspBenchmarkAgent>(a =>
            {
                a.InitializeAlgorithm(TspAlgorithmType.BruteForce);
                a.SetTimeLimit(timeLimit);
            })
            .AddAgent<TspBenchmarkAgent>(a =>
            {
                a.InitializeAlgorithm(TspAlgorithmType.HeldKarp);
                a.SetTimeLimit(timeLimit);
            })
            .AddAgent<TspBenchmarkAgent>(a =>
            {
                a.InitializeAlgorithm(TspAlgorithmType.MstPrim);
                a.SetTimeLimit(timeLimit);
            })
            .AddStage<PrepareNextTestCase>()
            .AddStage<RunAlgorithms>()
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
            .Build();

        return simulation;
    }
}

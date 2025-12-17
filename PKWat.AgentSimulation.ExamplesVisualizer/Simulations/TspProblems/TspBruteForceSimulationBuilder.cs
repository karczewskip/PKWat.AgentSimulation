namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.TspProblems;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Core.Crash;
using PKWat.AgentSimulation.Examples.TspProblems;
using PKWat.AgentSimulation.Examples.TspProblems.BruteForce.Stages;
using PKWat.AgentSimulation.Examples.TspProblems.Stages;
using System;
using System.Windows.Media.Imaging;

public class TspBruteForceSimulationBuilder(
    ISimulationBuilder simulationBuilder,
    TspBruteForceDrawer drawer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        drawer.Initialize(100, 100);

        var simulation = simulationBuilder
            .CreateNewSimulation<TspEnvironment>()
            .AddInitializationStage<InitializeTspSpace>(s => s.SetSize(100.0, 100.0))
            .AddInitializationStage<InitializeTspPoints>(s => s.SetPointCount(7))
            .AddInitializationStage<InitializeBruteForceAgent>()
            .AddStage<CheckNextPermutation>()
            .AddCallback(c => drawing(drawer.Draw(c)))
            .AddCrashCondition(c => 
            {
                var agent = c.GetAgents<Examples.TspProblems.Agents.BruteForceAgent>().FirstOrDefault();
                
                if (agent?.IsComplete == true)
                {
                    return SimulationCrashResult.Crash($"Brute Force Complete! Best distance: {agent.BestSolution?.TotalDistance:F2}");
                }
                
                if (c.Time.StepNo >= 50000)
                {
                    return SimulationCrashResult.Crash($"Max iterations reached. Best distance: {agent?.BestSolution?.TotalDistance:F2}");
                }
                
                return SimulationCrashResult.NoCrash;
            })
            .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(10))
            .Build();

        return simulation;
    }
}

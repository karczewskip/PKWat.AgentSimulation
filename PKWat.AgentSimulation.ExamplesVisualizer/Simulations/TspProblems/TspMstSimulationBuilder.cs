namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.TspProblems;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Core.Crash;
using PKWat.AgentSimulation.Examples.TspProblems;
using PKWat.AgentSimulation.Examples.TspProblems.Mst.Stages;
using PKWat.AgentSimulation.Examples.TspProblems.Stages;
using System;
using System.Windows.Media.Imaging;

public class TspMstSimulationBuilder(
    ISimulationBuilder simulationBuilder,
    TspMstDrawer drawer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        drawer.Initialize(100, 100);

        var simulation = simulationBuilder
            .CreateNewSimulation<TspEnvironment>()
            .AddInitializationStage<InitializeTspSpace>(s => s.SetSize(100.0, 100.0))
            .AddInitializationStage<InitializeTspPoints>(s => s.SetPointCount(8))
            .AddInitializationStage<InitializeMstAgent>()
            .AddInitializationStage<BuildMstWithPrim>()
            .AddStage<AddNextMstNode>()
            .AddCallback(c => drawing(drawer.Draw(c)))
            .AddCrashCondition(c => 
            {
                var agent = c.GetAgents<Examples.TspProblems.Agents.MstAgent>().FirstOrDefault();
                
                if (agent?.IsComplete == true)
                {
                    return SimulationCrashResult.Crash($"MST Approximation Complete! Best distance: {agent.BestSolution?.TotalDistance:F2}");
                }
                
                if (c.Time.StepNo >= 1000)
                {
                    return SimulationCrashResult.Crash($"Max iterations reached. Best distance: {agent?.BestSolution?.TotalDistance:F2}");
                }
                
                return SimulationCrashResult.NoCrash;
            })
            .SetRandomSeed(42)
            .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(100))
            .Build();

        return simulation;
    }
}

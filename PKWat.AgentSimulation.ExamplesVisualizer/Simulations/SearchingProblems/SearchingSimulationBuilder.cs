namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.SearchingProblems;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.SearchingProblems;
using PKWat.AgentSimulation.Examples.SearchingProblems.Stages;
using System;
using System.Windows.Media.Imaging;

public class SearchingSimulationBuilder(
    ISimulationBuilder simulationBuilder,
    SearchingDrawer searchingDrawer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        searchingDrawer.Initialize(100, 100);

        var crashCondition = new OptimalSolutionFoundCondition(optimalValueThreshold: 99.5, maxIterations: 1000);

        var simulation = simulationBuilder
            .CreateNewSimulation<SearchingEnvironment>()
            .AddInitializationStage<InitializeSearchSpace>(s => s.SetSize(100.0, 100.0))
            .AddInitializationStage<InitializeSearchPoints>()
            .AddInitializationStage<InitializeSearchAgents>()
            .AddStage<ExploreNextPoint>()
            .AddCallback(c => drawing(searchingDrawer.Draw(c)))
            .AddCrashCondition(crashCondition.CheckCondition)
            .SetRandomSeed(42)
            .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(50))
            .Build();

        return simulation;
    }
}

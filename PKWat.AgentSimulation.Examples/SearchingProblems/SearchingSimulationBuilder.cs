namespace PKWat.AgentSimulation.Examples.SearchingProblems;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.SearchingProblems.Stages;

public class SearchingSimulationBuilder
{
    private readonly ISimulationBuilder _simulationBuilder;

    public SearchingSimulationBuilder(ISimulationBuilder simulationBuilder)
    {
        _simulationBuilder = simulationBuilder;
    }

    public ISimulation Build(double optimalThreshold = 99.5, long maxIterations = 1000)
    {
        var crashCondition = new OptimalSolutionFoundCondition(optimalThreshold, maxIterations);

        var simulation = _simulationBuilder
            .CreateNewSimulation<SearchingEnvironment>()
            .AddInitializationStage<InitializeSearchSpace>(s => s.SetSize(100.0, 100.0))
            .AddInitializationStage<InitializeSearchPoints>()
            .AddInitializationStage<InitializeSearchAgents>()
            .AddStage<ExploreNextPoint>()
            .AddCrashCondition(crashCondition.CheckCondition)
            .SetRandomSeed(42)
            .Build();

        return simulation;
    }
}

namespace PKWat.AgentSimulation.Examples.SearchingProblems.Examples;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Example demonstrating how to create and run a searching algorithm simulation.
/// </summary>
public static class SearchingSimulationExample
{
    /// <summary>
    /// Creates and configures a basic searching simulation.
    /// </summary>
    /// <param name="simulationBuilder">The simulation builder instance.</param>
    /// <returns>A configured simulation ready to run.</returns>
    public static ISimulation CreateBasicSearchSimulation(ISimulationBuilder simulationBuilder)
    {
        var builder = new SearchingSimulationBuilder(simulationBuilder);
        return builder.Build(optimalThreshold: 99.5, maxIterations: 1000);
    }

    /// <summary>
    /// Creates a custom searching simulation with specific parameters.
    /// </summary>
    public static ISimulation CreateCustomSearchSimulation(
        ISimulationBuilder simulationBuilder,
        double searchSpaceWidth = 100.0,
        double searchSpaceHeight = 100.0,
        double optimalThreshold = 99.5,
        long maxIterations = 1000,
        int randomSeed = 42)
    {
        var crashCondition = new OptimalSolutionFoundCondition(optimalThreshold, maxIterations);

        return simulationBuilder
            .CreateNewSimulation<SearchingEnvironment>()
            .AddInitializationStage<Stages.InitializeSearchSpace>(s => s.SetSize(searchSpaceWidth, searchSpaceHeight))
            .AddInitializationStage<Stages.InitializeSearchPoints>()
            .AddInitializationStage<Stages.InitializeSearchAgents>()
            .AddStage<Stages.ExploreNextPoint>()
            .AddCrashCondition(crashCondition.CheckCondition)
            .SetRandomSeed(randomSeed)
            .Build();
    }

    /// <summary>
    /// Example of how to run the simulation with progress monitoring.
    /// </summary>
    public static async Task RunSearchSimulationWithMonitoring(ISimulationBuilder simulationBuilder)
    {
        var crashCondition = new OptimalSolutionFoundCondition(99.5, 1000);

        var simulation = simulationBuilder
            .CreateNewSimulation<SearchingEnvironment>()
            .AddInitializationStage<Stages.InitializeSearchSpace>(s => s.SetSize(100.0, 100.0))
            .AddInitializationStage<Stages.InitializeSearchPoints>()
            .AddInitializationStage<Stages.InitializeSearchAgents>()
            .AddStage<Stages.ExploreNextPoint>()
            .AddCallback(context =>
            {
                // Log progress every 100 iterations
                if (context.Time.StepNo % 100 == 0)
                {
                    var env = context.GetSimulationEnvironment<SearchingEnvironment>();
                    Console.WriteLine($"Iteration {context.Time.StepNo}: Best value = {env.BestPoint?.Value:F2} at ({env.BestPoint?.X:F2}, {env.BestPoint?.Y:F2})");
                }
            })
            .AddCrashCondition(crashCondition.CheckCondition)
            .SetRandomSeed(42)
            .Build();

        await simulation.StartAsync();

        if (simulation.Crash.IsCrash)
        {
            Console.WriteLine($"Simulation completed: {simulation.Crash.CrashReason}");
        }
    }
}

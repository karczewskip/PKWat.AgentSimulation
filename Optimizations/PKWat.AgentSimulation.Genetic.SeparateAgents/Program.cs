using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.Genetic.SeparateAgents.Logic;
using PKWat.AgentSimulation.Genetic.SeparateAgents.Logic.Stages;

var services = new ServiceCollection();
services.AddAgentSimulation(typeof(PolynomialCheckAgent).Assembly);

var serviceProvider = services.BuildServiceProvider();

var simulationBuilder = serviceProvider.GetRequiredService<ISimulationBuilder>();
var performanceInfo = serviceProvider.GetRequiredService<ISimulationCyclePerformanceInfo>();
performanceInfo.Subscribe(x => Console.WriteLine(x));

var simulation = simulationBuilder.CreateNewSimulation<CalculationsBlackboard>()
    .AddInitializationStage<InitializeBlackboard>()
    .AddStage<BuildNewAgents>()
    .AddStage<CalculateForAllAgents>()
    .AddCallback(c =>
    {
        var environment = c.GetSimulationEnvironment<CalculationsBlackboard>();

        Console.WriteLine($"Cycle {c.Time.StepNo}");

        foreach (var calculatedErrorsForSingleAgent in environment.AgentErrors.OrderBy(x => x.Value.AbsoluteError).Take(5))
        {
            Console.WriteLine($"Agent ID: {calculatedErrorsForSingleAgent.Key}, Calculated with error: {calculatedErrorsForSingleAgent.Value.AbsoluteError}");
        }
    })
    .Build();

await simulation.StartAsync();



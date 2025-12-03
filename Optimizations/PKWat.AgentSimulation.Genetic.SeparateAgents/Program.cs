using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.Genetics.PolynomialInterpolation;
using PKWat.AgentSimulation.Genetics.PolynomialInterpolation.Stages;

var services = new ServiceCollection();
services.AddAgentSimulation(typeof(PolynomialCheckAgent).Assembly);

var serviceProvider = services.BuildServiceProvider();

var simulationBuilder = serviceProvider.GetRequiredService<ISimulationBuilder>();
var performanceInfo = serviceProvider.GetRequiredService<ISimulationCyclePerformanceInfo>();
performanceInfo.Subscribe(x => Console.WriteLine(x));

var simulation = simulationBuilder.CreateNewSimulation<CalculationsBlackboard>()
    .AddInitializationStage<InitializeBlackboard>()
    .AddStage<BuildNewAgents>()
    .AddStage<CalculateForAllAgentsByGPU>()
    //.AddStage<CalculateForAllAgents>()
    //.SetRandomSeed(123)
    .AddCallback(c =>
    {
        var environment = c.GetSimulationEnvironment<CalculationsBlackboard>();

        Console.WriteLine($"Cycle {c.Time.StepNo}");

        var bestErrors = environment.AgentErrors.OrderBy(x => x.Value.AbsoluteError).Take(5);
        foreach (var calculatedErrorsForSingleAgent in bestErrors)
        {
            Console.WriteLine($"Agent ID: {calculatedErrorsForSingleAgent.Key}, Calculated with error: {calculatedErrorsForSingleAgent.Value.MeanAbsoluteError}");
        }

        var bestAgent = c.GetRequiredAgent<PolynomialCheckAgent>(bestErrors.First().Key);
        Console.WriteLine($"Best coeeficients: {bestAgent.Parameters}");

        
    })
    .Build();

await simulation.StartAsync();



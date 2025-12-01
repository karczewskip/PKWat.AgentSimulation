using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Genetic.SeparateAgents.Logic;

var services = new ServiceCollection();
services.AddAgentSimulation(typeof(PolynomialCheckAgent).Assembly);

var serviceProvider = services.BuildServiceProvider();

var simulationBuilder = serviceProvider.GetRequiredService<ISimulationBuilder>();

var simulation = simulationBuilder.CreateNewSimulationForDefaultEnvironment()
    .AddAgent<PolynomialCheckAgent>(x => x.SetParameters(PolynomialParameters.BuildFromCoefficients([1, 2, 3])))
    .AddAgent<PolynomialCheckAgent>(x => x.SetParameters(PolynomialParameters.BuildFromCoefficients([1, 2, 4])))
    .SetWaitingTimeBetweenSteps(TimeSpan.FromSeconds(1))
    .AddCallback(c =>
    {
        var agents = c.GetAgents<PolynomialCheckAgent>();
        var expectedValues = ExpectedValues.Build(Enumerable.Range(0, 50_000_000).Select(x => (double)x).ToArray(), x => x * x + 2 * x + 3);
        foreach (var agent in agents)
        {
            Console.WriteLine($"Agent ID: {agent.Id}, Calculated with error: {agent.CalculateError(expectedValues).AbsoluteError}");
        }
    })
    .Build();

await simulation.StartAsync();



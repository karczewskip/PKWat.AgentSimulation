
using PKWat.AgentSimulation.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Examples.Liquid.Simulation.Environment;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddAgentSimulation(typeof(Program).Assembly);

using IHost host = builder.Build();

var simultaionBuilder = host.Services.GetRequiredService<ISimulationBuilder>();

var simulaiton = simultaionBuilder.CreateNewSimulation(new LiquidEnvironment()).Build();

simulaiton.StartAsync();

// Wait 1 second
await Task.Delay(1000);

await simulaiton.StopAsync();
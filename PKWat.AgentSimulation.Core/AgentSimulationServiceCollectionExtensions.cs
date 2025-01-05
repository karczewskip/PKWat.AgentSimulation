namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using System.Reflection;

public static class AgentSimulationServiceCollectionExtensions
{
    public static void AddAgentSimulation(this IServiceCollection services, Assembly assembly)
    {
        services.AddSingleton<ISimulationBuilder, SimulationBuilder>();
        services.AddScoped<RandomNumbersGeneratorFactory>();
        services.AddTransient(s => s.GetRequiredService<RandomNumbersGeneratorFactory>().Create());
        services.AddScoped<SimulationPerformanceInfo>();
        services.AddScoped<ISimulationCyclePerformanceInfo>(c => c.GetRequiredService<SimulationPerformanceInfo>());

        Type[] registeringGenericTypes = [typeof(ISimulationEnvironment), typeof(ISimulationAgent), typeof(ISimulationEvent)];

        foreach (var type in assembly.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface))
        {
            var interfaces = type.GetInterfaces();
            if(interfaces.Any(i => registeringGenericTypes.Contains(i)))
            {
                services.AddTransient(type);
            }
        }
    }
}

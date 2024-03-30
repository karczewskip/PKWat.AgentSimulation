namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public static class AgentSimulationServiceCollectionExtensions
{
    public static void AddAgentSimulation(this IServiceCollection services, Assembly assembly)
    {
        services.AddSingleton<ISimulationBuilder, SimulationBuilder>();
        services.AddScoped<RandomNumbersGeneratorFactory>();
        services.AddTransient(s => s.GetRequiredService<RandomNumbersGeneratorFactory>().Create());

        foreach (var type in assembly.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface))
        {
            var interfaces = type.GetInterfaces();
            if(interfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISimulationAgent<>)))
            {
                services.AddTransient(type);
            }
        }


    }
}

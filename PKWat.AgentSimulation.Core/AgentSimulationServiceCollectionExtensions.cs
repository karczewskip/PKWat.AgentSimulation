namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Core.Time;
using System.Reflection;

public static class AgentSimulationServiceCollectionExtensions
{
    public static void AddAgentSimulation(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddSingleton<ISimulationBuilder, SimulationBuilder>();
        services.AddScoped<RandomNumbersGeneratorFactory>();
        services.AddTransient(s => s.GetRequiredService<RandomNumbersGeneratorFactory>().Create());
        services.AddScoped<SimulationPerformanceInfo>();
        services.AddScoped<SimulationCalendar>();
        services.AddScoped<ISimulationCalendar>(c => c.GetRequiredService<SimulationCalendar>());
        services.AddScoped<ISimulationCalendarScheduler>(c => c.GetRequiredService<SimulationCalendar>());
        services.AddScoped<ISimulationCyclePerformanceInfo>(c => c.GetRequiredService<SimulationPerformanceInfo>());

        Type[] registeringGenericTypes = [
            typeof(ISimulationEnvironment), 
            typeof(ISimulationAgent), 
            typeof(ISimulationStage)];

        foreach (var type in assemblies.SelectMany(x => x.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface)))
        {
            var interfaces = type.GetInterfaces();
            if(interfaces.Any(i => registeringGenericTypes.Contains(i)))
            {
                services.AddTransient(type);
            }
        }
    }
}

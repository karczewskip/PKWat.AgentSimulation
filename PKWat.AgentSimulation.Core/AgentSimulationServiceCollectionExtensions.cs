namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;

public static class AgentSimulationServiceCollectionExtensions
{
    public static void AddAgentSimulation(this IServiceCollection services)
    {
        services.AddSingleton<ISimulationBuilder, SimulationBuilder>();
    }
}

namespace PKWat.AgentSimulation.Core;

using System.Threading.Tasks;

public interface ISimulationEvent<ENVIRONMENT>
{
    Task<bool> ShouldBeExecuted(ISimulationContext<ENVIRONMENT> context);
    Task Execute(ISimulationContext<ENVIRONMENT> context);
}

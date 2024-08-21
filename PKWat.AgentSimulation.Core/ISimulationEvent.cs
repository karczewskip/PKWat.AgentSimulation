namespace PKWat.AgentSimulation.Core;

using PKWat.AgentSimulation.Core.Snapshots;
using System.Threading.Tasks;

public interface ISimulationEvent<ENVIRONMENT> where ENVIRONMENT : ISimulationEnvironment
{
    Task<bool> ShouldBeExecuted(ISimulationContext<ENVIRONMENT> context);
    Task Execute(ISimulationContext<ENVIRONMENT> context);
}

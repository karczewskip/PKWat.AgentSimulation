namespace PKWat.AgentSimulation.Core;

using System.Threading.Tasks;

public interface ISimulationEvent
{

}

public interface ISimulationEvent<ENVIRONMENT, ENVIRONMENT_STATE> : ISimulationEvent where ENVIRONMENT : ISimulationEnvironment<ENVIRONMENT_STATE>
{
    Task<bool> ShouldBeExecuted(ISimulationContext<ENVIRONMENT, ENVIRONMENT_STATE> context);
    Task Execute(ISimulationContext<ENVIRONMENT, ENVIRONMENT_STATE> context);
}

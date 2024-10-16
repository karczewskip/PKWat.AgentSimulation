namespace PKWat.AgentSimulation.Core;

using System.Threading.Tasks;

public interface ISimulationEvent
{

}

public interface ISimulationEvent<ENVIRONMENT> : ISimulationEvent where ENVIRONMENT : ISimulationEnvironment
{
    Task<bool> ShouldBeExecuted(ISimulationContext<ENVIRONMENT> context);
    Task Execute(ISimulationContext<ENVIRONMENT> context);
}

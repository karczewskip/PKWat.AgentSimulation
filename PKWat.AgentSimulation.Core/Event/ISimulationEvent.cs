namespace PKWat.AgentSimulation.Core.Event;

public interface ISimulationEvent
{
    Task Execute(ISimulationContext context);
}

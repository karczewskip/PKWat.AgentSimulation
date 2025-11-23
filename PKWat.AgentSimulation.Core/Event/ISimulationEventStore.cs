namespace PKWat.AgentSimulation.Core.Event;

public interface ISimulationEventStore
{
    public void ScheduleEventAt<U>(TimeSpan moment) where U : ISimulationEvent;
    public void ScheduleEventAt<U>(TimeSpan moment, Action<U> initialization) where U : ISimulationEvent;
}

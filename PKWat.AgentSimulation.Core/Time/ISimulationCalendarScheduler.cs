namespace PKWat.AgentSimulation.Core.Time;

public interface ISimulationCalendarScheduler
{
    void ScheduleNewStepAt(TimeSpan moment);
    void ScheduleNewStepAfter(TimeSpan delay);
}

using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core.Time;

namespace PKWat.AgentSimulation.Core.Event;

internal class SimulationEventStore(IServiceProvider serviceProvider, ISimulationCalendarScheduler simulationCalendarScheduler) : ISimulationEventStore
{
    private readonly SortedDictionary<TimeSpan, List<ISimulationEvent>> _scheduledEvents 
        = new();

    public void ScheduleEventAt<U>(TimeSpan moment) where U : ISimulationEvent
    {
        ScheduleEventAt<U>(moment, _ => { });
    }

    public void ScheduleEventAt<U>(TimeSpan moment, Action<U> initialization) where U : ISimulationEvent
    {
        if (!_scheduledEvents.ContainsKey(moment))
        {
            _scheduledEvents[moment] = new List<ISimulationEvent>();
        }
        var newEvent = serviceProvider.GetRequiredService<U>();
        initialization(newEvent);
        _scheduledEvents[moment].Add(newEvent);

        simulationCalendarScheduler.ScheduleNewStepAt(moment);
    }

    public List<ISimulationEvent> GetAndRemoveEventsAt(TimeSpan moment)
    {
        if (_scheduledEvents.TryGetValue(moment, out var events))
        {
            _scheduledEvents.Remove(moment);
            return events;
        }
        return new List<ISimulationEvent>();
    }
}

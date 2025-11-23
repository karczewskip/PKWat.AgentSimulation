namespace PKWat.AgentSimulation.Core.Time;

internal interface ISimulationCalendar
{
    public bool HaveNextStep { get; }
    public TimeSpan MoveToNextStep();
}

internal class SimulationCalendar : ISimulationCalendar, ISimulationCalendarScheduler
{
    private readonly List<TimeSpan> orderedTimes = new();

    private TimeSpan currentTime = TimeSpan.Zero;

    public bool HaveNextStep => orderedTimes.Count > 0;

    public TimeSpan MoveToNextStep()
    {
        if (!HaveNextStep) throw new InvalidOperationException("No more time steps available.");

        var nextStep = orderedTimes.First();
        orderedTimes.RemoveAt(0);
        currentTime = nextStep;
        return nextStep;
    }

    public void ScheduleNewStepAfter(TimeSpan delay)
    {
        var newStepTime = currentTime + delay;
        ScheduleNewStepAt(newStepTime);
    }

    public void ScheduleNewStepAt(TimeSpan moment)
    {
        int index = orderedTimes.BinarySearch(moment);
        if (index < 0)
        {
            index = ~index; // Get the insertion point
            orderedTimes.Insert(index, moment);
        }
    }

}

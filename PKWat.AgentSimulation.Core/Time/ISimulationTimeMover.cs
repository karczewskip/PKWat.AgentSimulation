namespace PKWat.AgentSimulation.Core.Time;

internal interface ISimulationTimeMover : ISimulationTimeProvider
{
    void MoveSimulationTime();
    void ResetTime();
}

internal class IntervalBasedSimulationTimeMover(TimeSpan stepInterval) : ISimulationTimeMover
{
    private SimulationTime _currentTime = SimulationTime.CreateZero();

    public IReadOnlySimulationTime Time => _currentTime;
    public void MoveSimulationTime()
    {
        _currentTime = _currentTime.AddStep(stepInterval);
    }

    public void ResetTime()
    {
        _currentTime = SimulationTime.CreateZero();
    }
}

internal class CalendarSimulationTimeMover(ISimulationCalendar calendar) : ISimulationTimeMover
{
    private SimulationTime _currentTime = SimulationTime.CreateZero();

    public IReadOnlySimulationTime Time => _currentTime;

    public void MoveSimulationTime()
    {
        if (calendar.HaveNextStep)
        {
            var timeSpan = calendar.MoveToNextStep();
            _currentTime = _currentTime.SetTime(timeSpan);
        }
    }

    public void ResetTime()
    {
        _currentTime = SimulationTime.CreateZero();
    }
}

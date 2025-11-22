namespace PKWat.AgentSimulation.Core.Time;

internal interface ISimulationCalendar
{
    public bool HaveNextStep { get; }
    public SimulationTime MoveToNextStep();
}

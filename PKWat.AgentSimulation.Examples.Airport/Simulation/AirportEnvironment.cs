namespace PKWat.AgentSimulation.Examples.Airport.Simulation;

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Environment;
using System;

public class AirportEnvironment : DefaultSimulationEnvironment
{
    public TimeSpan? NewAirplaneArrival { get; private set; }
    public bool IsNewAirplaneArrivalScheduled => NewAirplaneArrival.HasValue;

    public Queue<AgentId> WaitingAirplanes { get; } = new();

    public int[] AllLandingLines { get; private set; } = new int[] { 1 };

    internal void SetLandingLines(int[] landingLines)
    {
        AllLandingLines = landingLines;
    }

    internal void ScheduleNewAirplaneArrival(TimeSpan nextArrival)
    {
        NewAirplaneArrival = nextArrival;
    }

    internal void AddAirplaneToWaitingList(AgentId id)
    {
        WaitingAirplanes.Enqueue(id);
    }
}

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Environment;
using System;

namespace PKWat.AgentSimulation.Examples.Airport3;

public class AirportEnvironment : DefaultSimulationEnvironment
{

    public Queue<AgentId> WaitingAirplanes { get; } = new();
    public Queue<int> AvailableLines { get; } = new();

    public int[] AllLandingLines { get; private set; } = new int[0];

    internal void SetLandingLines(int[] landingLines)
    {
        AllLandingLines = landingLines;
        foreach (var line in landingLines)
        {
            AvailableLines.Enqueue(line);
        }
    }

    internal void AddAirplaneToWaitingList(AgentId id)
    {
        WaitingAirplanes.Enqueue(id);
    }
}

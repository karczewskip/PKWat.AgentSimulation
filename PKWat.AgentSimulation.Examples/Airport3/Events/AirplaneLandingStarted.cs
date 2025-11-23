using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Event;
using PKWat.AgentSimulation.Examples.Airport3.Agents;

namespace PKWat.AgentSimulation.Examples.Airport3.Events;

internal class AirplaneLandingStarted : ISimulationEvent
{
    private AgentId? _airplaneId = null;

    public void SetAirplane(AgentId airplaneId)
    {
        _airplaneId = airplaneId;
    }

    public async Task Execute(ISimulationContext context)
    {
        var landingTime = TimeSpan.FromMinutes(10);
        if (_airplaneId != null)
        {
            var airplane = context.GetRequiredAgent<Airplane>(_airplaneId);
            var start = context.Time.Time;
            var end = start + landingTime;
            airplane.StartLanding(start, end);
            return;
        }

        var airplanes = context.GetAgents<Airplane>().Where(x => x.WaitsForLanding && x.AssignedLine.HasValue);
        foreach (var airplane in airplanes)
        {
            var start = context.Time.Time;
            var end = start + landingTime;
            airplane.StartLanding(start, end);
        }
    }
}

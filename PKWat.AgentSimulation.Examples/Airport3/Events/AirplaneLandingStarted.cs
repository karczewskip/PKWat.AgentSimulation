using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Event;
using PKWat.AgentSimulation.Examples.Airport3.Agents;

namespace PKWat.AgentSimulation.Examples.Airport3.Events;

internal class AirplaneLandingStarted(ISimulationEventStore simulationEventStore) : ISimulationEvent
{
    private AgentId? _airplaneId = null;

    public void SetAirplane(AgentId airplaneId)
    {
        _airplaneId = airplaneId;
    }

    public async Task Execute(ISimulationContext context)
    {
        if(_airplaneId == null)
        {
            throw new InvalidOperationException("Airplane ID is not set.");
        }

        var landingTime = TimeSpan.FromMinutes(10);
        if (_airplaneId != null)
        {
            var airplane = context.GetRequiredAgent<Airplane>(_airplaneId);
            var start = context.Time.Time;
            var end = start + landingTime;
            airplane.StartLanding(start, end);
            simulationEventStore.ScheduleEventAt<AirplaneLandingFinished>(end, e => e.SetAirplane(airplane.Id));
            return;
        }

        
    }
}

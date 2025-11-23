using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Event;
using PKWat.AgentSimulation.Examples.Airport3.Agents;

namespace PKWat.AgentSimulation.Examples.Airport3.Events;

internal class AssignedNextAirplaneToLine(ISimulationEventStore simulationEventStore) : ISimulationEvent
{
    private AgentId? airplaneId = null;

    public void SetAirplaneId(AgentId airplaneId)
        => this.airplaneId = airplaneId;

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<AirportEnvironment>();

        if (!environment.AvailableLines.Any())
            return;

        var line = environment.AvailableLines.Dequeue();

        if (airplaneId != null)
        {
            var airplane = context.GetRequiredAgent<Airplane>(airplaneId);
            airplane.AssignedLine = line;
            simulationEventStore.ScheduleEventAt<AirplaneLandingStarted>(TimeSpan.FromMinutes(1), e => e.SetAirplane(airplane.Id));
            return;
        }

        if (environment.WaitingAirplanes.Any())
        {
            var airplaneId = environment.WaitingAirplanes.Dequeue();
            
            var airplane = context.GetRequiredAgent<Airplane>(airplaneId);
            airplane.AssignedLine = line;

            simulationEventStore.ScheduleEventAt<AirplaneLandingStarted>(TimeSpan.FromMinutes(1), e => e.SetAirplane(airplane.Id));
        }
    }
}

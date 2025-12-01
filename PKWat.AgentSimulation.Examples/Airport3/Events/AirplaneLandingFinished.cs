using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Event;
using PKWat.AgentSimulation.Examples.Airport3.Agents;

namespace PKWat.AgentSimulation.Examples.Airport3.Events;

internal class AirplaneLandingFinished(ISimulationEventStore simulationEventStore) : ISimulationEvent
{
    private AgentId? _airplaneId = null;

    public void SetAirplane(AgentId airplaneId)
    {
        _airplaneId = airplaneId;
    }

    public async Task Execute(ISimulationContext context)
    {
        if (_airplaneId == null)
            throw new InvalidOperationException("Airplane ID is not set.");
        
        var airplane = context.GetRequiredAgent<Airplane>(_airplaneId);
        if (airplane.PassengersInAirplane.Any())
        {
            var passengerId = airplane.PassengersInAirplane.Dequeue();
            var startCheckouting = context.Time.Time;
            var passenger = context.GetRequiredAgent<Passenger>(passengerId);
            var plannedCheckoutTime = passenger.StartCheckout(startCheckouting);
            airplane.PassengerCheckoutBlockTime = plannedCheckoutTime;

            simulationEventStore.ScheduleEventAt<PassengerCheckouted>(
                plannedCheckoutTime,
                evt => evt.SetPassengerId(passengerId)
            );
        
        }
    }
}

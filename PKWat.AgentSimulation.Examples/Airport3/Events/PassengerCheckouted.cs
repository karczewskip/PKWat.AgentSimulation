using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Event;
using PKWat.AgentSimulation.Examples.Airport3.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKWat.AgentSimulation.Examples.Airport3.Events;

internal class PassengerCheckouted(ISimulationEventStore simulationEventStore) : ISimulationEvent
{
    private AgentId? passengerId;

    public void SetPassengerId(AgentId id)
    {
        passengerId = id;
    }

    public async Task Execute(ISimulationContext context)
    {
        if (passengerId == null)
        {
            throw new InvalidOperationException("PassengerId is not set.");
        }
        var passenger = context.GetRequiredAgent<Passenger>(passengerId);
        var airplane = context.GetRequiredAgent<Airplane>(passenger.AirplaneId);

        if(airplane.PassengersInAirplane.Any())
        {
            var nextPassengerId = airplane.PassengersInAirplane.Dequeue();
            
            var nextPassenger = context.GetRequiredAgent<Passenger>(nextPassengerId);
            var now = context.Time.Time;
            var checkoutTime = nextPassenger.StartCheckout(now);

            simulationEventStore.ScheduleEventAt<PassengerCheckouted>(checkoutTime, (e) =>
            {
                e.SetPassengerId(nextPassengerId);
            });
        }
        else
        {
            // departure of the airplane can be scheduled here
        }
    }
}

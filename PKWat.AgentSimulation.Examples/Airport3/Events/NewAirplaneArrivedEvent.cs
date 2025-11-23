using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Event;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Examples.Airport3.Agents;

namespace PKWat.AgentSimulation.Examples.Airport3.Events;

public class NewAirplaneArrivedEvent(IRandomNumbersGenerator randomNumbersGenerator, ISimulationEventStore simulationEventStore) : ISimulationEvent
{
    private int maxNumberOfPassengers = 30;
    private double meanTimeBetweenArrivalsInMinutes = 10.0;

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<AirportEnvironment>();

        AddNewAirplane(context);
        ScheduleNextAirplaneArrival(context);
    }

    private void AddNewAirplane(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<AirportEnvironment>();

        var newAirplane = context.AddAgent<Airplane>();
        var passengers = randomNumbersGenerator.Next(maxNumberOfPassengers);
        for (int i = 0; i < passengers; i++)
        {
            var passanger = context.AddAgent<Passenger>();
            passanger.SetAirplane(newAirplane.Id);
            newAirplane.PassengersInAirplane.Enqueue(passanger.Id);
        }
        environment.AddAirplaneToWaitingList(newAirplane.Id);
    }

    private void ScheduleNextAirplaneArrival(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<AirportEnvironment>();

        var nextArrival = context.Time.Time
            + TimeSpan.FromMinutes(randomNumbersGenerator.GetNextExponential(1.0 / meanTimeBetweenArrivalsInMinutes));

        simulationEventStore.ScheduleEventAt<NewAirplaneArrivedEvent>(nextArrival);
    }
}


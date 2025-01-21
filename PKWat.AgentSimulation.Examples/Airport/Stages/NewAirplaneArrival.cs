namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Agents;
using System;
using System.Threading.Tasks;

public class NewAirplaneArrival(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage
{
    private int maxNumberOfPassengers = 30;
    private double meanTimeBetweenArrivalsInMinutes = 10.0;

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<AirportEnvironment>();

        if (environment.IsNewAirplaneArrivalScheduled
            && environment.NewAirplaneArrival < context.Time.Time)
        {
            var previousArrival = environment.NewAirplaneArrival;

            AddNewAirplane(context);
            ScheduleNextAirplaneArrival(context, previousArrival);
        }
        else if (!environment.IsNewAirplaneArrivalScheduled)
        {
            ScheduleNextAirplaneArrival(context);
        }
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

    private void ScheduleNextAirplaneArrival(ISimulationContext context, TimeSpan? lastArrival = null)
    {
        var environment = context.GetSimulationEnvironment<AirportEnvironment>();

        var nextArrival = (lastArrival ?? context.Time.Time)
            + TimeSpan.FromMinutes(randomNumbersGenerator.GetNextExponential(1.0 / meanTimeBetweenArrivalsInMinutes));

        environment.ScheduleNewAirplaneArrival(nextArrival);
    }
}

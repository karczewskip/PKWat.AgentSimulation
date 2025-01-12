namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;
using System;
using System.Threading.Tasks;

internal class NewAirplaneArrival(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage<AirportEnvironment>
{
    private int maxNumberOfPassengers = 30;
    private double meanTimeBetweenArrivalsInMinutes = 10.0;

    public async Task Execute(ISimulationContext<AirportEnvironment> context)
    {
        if(context.SimulationEnvironment.IsNewAirplaneArrivalScheduled 
            && context.SimulationEnvironment.NewAirplaneArrival < context.SimulationTime.Time)
        {
            var previousArrival = context.SimulationEnvironment.NewAirplaneArrival;

            AddNewAirplane(context);
            ScheduleNextAirplaneArrival(context, previousArrival);
        }
        else if(!context.SimulationEnvironment.IsNewAirplaneArrivalScheduled)
        {
            ScheduleNextAirplaneArrival(context);
        }
    }

    private void AddNewAirplane(ISimulationContext<AirportEnvironment> context)
    {
        var newAirplane = context.AddAgent<Airplane>();
        var passengers = randomNumbersGenerator.Next(maxNumberOfPassengers);
        for (int i = 0; i < passengers; i++)
        {
            var passanger = context.AddAgent<Passenger>();
            passanger.SetAirplane(newAirplane.Id);
            newAirplane.PassengersInAirplane.Enqueue(passanger.Id);
        }
        context.SimulationEnvironment.AddAirplaneToWaitingList(newAirplane.Id);
    }

    private void ScheduleNextAirplaneArrival(ISimulationContext<AirportEnvironment> context, TimeSpan? lastArrival = null)
    {
        var nextArrival = (lastArrival ?? context.SimulationTime.Time)
            + TimeSpan.FromMinutes(randomNumbersGenerator.GetNextExponential(1.0 / meanTimeBetweenArrivalsInMinutes));

        context.SimulationEnvironment.ScheduleNewAirplaneArrival(nextArrival);
    }
}

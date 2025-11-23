using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Core.Time;
using PKWat.AgentSimulation.Examples.Airport2.Agents;

namespace PKWat.AgentSimulation.Examples.Airport2.Stages;


public class StartCheckoutingPassengers(ISimulationCalendarScheduler simulationCalendarScheduler) : ISimulationStage
{
    private TimeSpan checkoutTime = TimeSpan.FromMinutes(5);

    public void SetCheckoutTime(TimeSpan checkoutTime)
    {
        this.checkoutTime = checkoutTime;
    }

    public async Task Execute(ISimulationContext context)
    {
        var simulationTime = context.Time;

        foreach (var airplane in context.GetAgents<Airplane>().Where(x => x.IsLanded(simulationTime.Time)))
        {
            if (airplane.PassengerCheckoutBlockTime.HasValue && airplane.PassengerCheckoutBlockTime.Value > simulationTime.Time)
            {
                continue;
            }

            if (airplane.PassengersInAirplane.Any())
            {
                var passengerId = airplane.PassengersInAirplane.Dequeue();
                var startCheckouting = simulationTime.Time;
                var plannedCheckoutTime = simulationTime.Time + checkoutTime;
                var passenger = context.GetRequiredAgent<Passenger>(passengerId);
                passenger.StartCheckout(startCheckouting, plannedCheckoutTime);
                airplane.PassengerCheckoutBlockTime = plannedCheckoutTime;

                simulationCalendarScheduler.ScheduleNewStepAt(plannedCheckoutTime);
            }
        }
    }
}


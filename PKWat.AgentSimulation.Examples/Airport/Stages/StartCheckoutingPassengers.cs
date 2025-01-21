namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Agents;
using System.Linq;
using System.Threading.Tasks;

public class StartCheckoutingPassengers : ISimulationStage
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
            }
        }
    }
}

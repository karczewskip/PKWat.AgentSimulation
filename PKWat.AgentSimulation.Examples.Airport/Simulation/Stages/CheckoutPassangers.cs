namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;
using System.Linq;
using System.Threading.Tasks;

internal class CheckoutPassangers : ISimulationStage<AirportEnvironment>
{
    public async Task Execute(ISimulationContext<AirportEnvironment> context)
    {
        var simulationTime = context.SimulationTime;
        var passangers = context.GetAgents<Passenger>().Where(x => x.IsBeforeCheckout);
        foreach (var passenger in passangers)
        {
            var airplane = context.GetRequiredAgent<Airplane>(passenger.AirplaneId);
            if (airplane.IsLanded(simulationTime.Time))
            {
                var checkoutTime = simulationTime.Time;
                var plannedCheckoutTime = checkoutTime + simulationTime.Step * 3;
                passenger.StartCheckout(checkoutTime, plannedCheckoutTime);
            }
        }
    }
}

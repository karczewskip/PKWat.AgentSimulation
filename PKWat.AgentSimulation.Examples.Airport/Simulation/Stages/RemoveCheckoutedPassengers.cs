namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;
using System.Linq;
using System.Threading.Tasks;

internal class RemoveCheckoutedPassengers : ISimulationStage<AirportEnvironment>
{
    public async Task Execute(ISimulationContext<AirportEnvironment> context)
    {
        var checkoutedPassengers = context.GetAgents<Passenger>()
            .Where(p => p.IsCheckouted(context.SimulationTime.Time));

        foreach (var passenger in checkoutedPassengers)
        {
            context.RemoveAgent(passenger.Id);
        }
    }
}

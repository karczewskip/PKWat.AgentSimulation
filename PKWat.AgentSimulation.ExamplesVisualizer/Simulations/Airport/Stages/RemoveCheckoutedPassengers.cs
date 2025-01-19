namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Agents;
using System.Linq;
using System.Threading.Tasks;

internal class RemoveCheckoutedPassengers : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var checkoutedPassengers = context.GetAgents<Passenger>()
            .Where(p => p.IsCheckouted(context.Time.Time));

        foreach (var passenger in checkoutedPassengers)
        {
            context.RemoveAgent(passenger.Id);
        }
    }
}

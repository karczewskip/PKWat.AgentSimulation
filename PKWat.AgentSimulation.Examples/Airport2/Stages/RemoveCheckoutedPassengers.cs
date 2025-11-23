using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.Airport2.Agents;

namespace PKWat.AgentSimulation.Examples.Airport2.Stages;

public class RemoveCheckoutedPassengers : ISimulationStage
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

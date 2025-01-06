namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Events;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Event;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;
using System.Linq;
using System.Threading.Tasks;

internal class PredatorsDied : ISimulationEvent<PreyVsPredatorEnvironment>
{
    public async Task Execute(ISimulationContext<PreyVsPredatorEnvironment> context)
    {
        var deadPredators = context.GetAgents<Predator>().Where(x => x.State.Health.Died).ToArray();
        foreach (var predator in deadPredators)
        {
            context.SimulationEnvironment.RemovePredator(predator.Id);
            context.RemoveAgent(predator.Id);
        }
    }

    public async Task<bool> ShouldBeExecuted(ISimulationContext<PreyVsPredatorEnvironment> context)
    {
        return true;
    }
}

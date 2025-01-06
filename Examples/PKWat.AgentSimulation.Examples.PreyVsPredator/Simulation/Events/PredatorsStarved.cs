namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Events;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Event;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;
using System.Threading.Tasks;

internal class PredatorsStarved : ISimulationEvent<PreyVsPredatorEnvironment>
{
    private double starvationIncrement = 0.0008;

    public void ChangeStarvationIncrement(double newIncrement)
    {
        starvationIncrement = newIncrement;
    }

    public async Task Execute(ISimulationContext<PreyVsPredatorEnvironment> context)
    {
        var deadPredators = new List<Predator>();
        var allPredators = context.GetAgents<Predator>();
        foreach (var predator in allPredators)
        {
            var newHealth = predator.DecreaseHealth(starvationIncrement);
            if (newHealth.Died)
            {
                deadPredators.Add(predator);
            }
        }

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

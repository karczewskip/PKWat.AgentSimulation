namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator.Agents;
using System.Threading.Tasks;

internal class PredatorsStarved(ISimulationCyclePerformanceInfo simulationCyclePerformanceInfo) : ISimulationStage
{
    private double starvationIncrement = 0.0008;

    public void ChangeStarvationIncrement(double newIncrement)
    {
        starvationIncrement = newIncrement;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<PreyVsPredatorEnvironment>();

        using var _ = simulationCyclePerformanceInfo.AddStep("PredatorsStarved");
        var deadPredators = new List<Predator>();
        var allPredators = context.GetAgents<Predator>();
        foreach (var predator in allPredators)
        {
            predator.DecreaseHealth(starvationIncrement);
            if (predator.IsDied)
            {
                deadPredators.Add(predator);
            }
        }

        foreach (var predator in deadPredators)
        {
            environment.RemovePredator(predator.Id);
            context.RemoveAgent(predator.Id);
        }
    }
}

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator.Agents;
using System.Threading.Tasks;

internal class PreyersEaten(ISimulationCyclePerformanceInfo simulationCyclePerformanceInfo) : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<PreyVsPredatorEnvironment>();

        using var step = simulationCyclePerformanceInfo.AddStep("PreyersEaten");
        var newBornPredators = new List<(AgentId NewBorn, AgentId Parent)>();
        var predators = context.GetAgents<Predator>().ToArray();

        foreach (var predator in predators)
        {
            var eatenPrey = environment.EatPreyByPredator(predator.Id);
            if (eatenPrey != null)
            {
                context.RemoveAgent(eatenPrey);

                var newBornPredator = context.AddAgent<Predator>();
                newBornPredators.Add((newBornPredator.Id, predator.Id));

                predator.ResetAfterEaten();
            }
        }

        environment.PlaceNewBornPredators(newBornPredators);
    }
}

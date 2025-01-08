namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;
using System.Threading.Tasks;

internal class PreyersEaten(ISimulationCyclePerformanceInfo simulationCyclePerformanceInfo) : ISimulationStage<PreyVsPredatorEnvironment>
{
    public async Task Execute(ISimulationContext<PreyVsPredatorEnvironment> context)
    {
        using var step = simulationCyclePerformanceInfo.AddStep("PreyersEaten");
        var newBornPredators = new List<(AgentId NewBorn, AgentId Parent)>();
        var predators = context.GetAgents<Predator>().ToArray();

        foreach (var predator in predators)
        {
            var eatenPrey = context.SimulationEnvironment.EatPreyByPredator(predator.Id);
            if (eatenPrey != null)
            {
                context.RemoveAgent(eatenPrey);

                var newBornPredator = context.AddAgent<Predator>();
                newBornPredators.Add((newBornPredator.Id, predator.Id));

                predator.ResetAfterEaten();
            }
        }

        context.SimulationEnvironment.PlaceNewBornPredators(newBornPredators);
    }
}

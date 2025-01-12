namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
using System.Threading.Tasks;

internal class ChangeAfterVisitFood : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<ColonyEnvironment>();

        var food = environment.FoodSource;
        foreach (var ant in context.GetAgents<Ant>())
        {
            foreach (var foodItem in food)
            {
                if (ant.Coordinates.IsInRange(foodItem.Coordinates, foodItem.SizeRadius))
                {
                    ant.GetFood();
                }
            }
        }
    }
}

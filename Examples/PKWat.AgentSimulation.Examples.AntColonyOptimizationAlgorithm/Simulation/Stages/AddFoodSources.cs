namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
using System.Collections.Generic;
using System.Threading.Tasks;

internal class AddFoodSources : ISimulationStage<ColonyEnvironment>
{
    private List<FoodSource> foodSources = new List<FoodSource>();
    public void AddFoodSource(FoodSource foodSource)
    {
        foodSources.Add(foodSource);
    }
    public async Task Execute(ISimulationContext<ColonyEnvironment> context)
    {
        foreach (var foodSource in foodSources)
        {
            context.SimulationEnvironment.FoodSource.Add(foodSource);
        }
    }
}
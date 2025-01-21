namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AddFoodSources : ISimulationStage
{
    private List<FoodSource> foodSources = new List<FoodSource>();
    public void AddFoodSource(FoodSource foodSource)
    {
        foodSources.Add(foodSource);
    }
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<ColonyEnvironment>();

        foreach (var foodSource in foodSources)
        {
            environment.FoodSource.Add(foodSource);
        }
    }
}
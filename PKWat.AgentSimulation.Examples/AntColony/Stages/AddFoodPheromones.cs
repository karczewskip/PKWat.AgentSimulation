namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony;
using System;
using System.Linq;
using System.Threading.Tasks;

public class AddFoodPheromones : ISimulationStage
{
    private double pheromonesPersistence = 8;

    public void SetPheromonesPersistence(double pheromonesPersistence)
    {
        this.pheromonesPersistence = pheromonesPersistence;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<ColonyEnvironment>();

        foreach (var agent in context.GetAgents<Ant>().Where(x => x.IsCarryingFood))
        {
            var agentFoodPheromones = Pheromones.MaxPheromoneValue * Math.Exp(-agent.PathLength / pheromonesPersistence);
            environment.Pheromones[agent.Coordinates.X, agent.Coordinates.Y].AddFood(agentFoodPheromones);
        }

        foreach (var food in environment.FoodSource)
        {
            for (var x = food.Coordinates.X - food.SizeRadius; x <= food.Coordinates.X + food.SizeRadius; x++)
            {
                for (var y = food.Coordinates.Y - food.SizeRadius; y <= food.Coordinates.Y + food.SizeRadius; y++)
                {
                    if (food.Coordinates.IsInRange(x, y, food.SizeRadius))
                    {
                        environment.Pheromones[(int)x, (int)y].AddFood(Pheromones.MaxPheromoneValue);
                    }
                }
            }
        }
    }
}
namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

internal class AddFoodPheromones : ISimulationStage<ColonyEnvironment>
{
    public async Task Execute(ISimulationContext<ColonyEnvironment> context)
    {
        foreach(var agent in context.GetAgents<Ant>().Where(x => x.IsCarryingFood))
        {
            var agentFoodPheromones = Pheromones.MaxPheromoneValue*Math.Exp(-agent.PathLength/8);
            context.SimulationEnvironment.Pheromones[agent.Coordinates.X, agent.Coordinates.Y].AddFood(agentFoodPheromones);
        }
    }
}
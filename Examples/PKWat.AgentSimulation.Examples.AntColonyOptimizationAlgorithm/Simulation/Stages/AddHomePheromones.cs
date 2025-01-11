namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

internal class AddHomePheromones : ISimulationStage<ColonyEnvironment>
{
    private double pheromonesPersistence = 8;

    public void SetPheromonesPersistence(double pheromonesPersistence)
    {
        this.pheromonesPersistence = pheromonesPersistence;
    }

    public async Task Execute(ISimulationContext<ColonyEnvironment> context)
    {
        foreach (var agent in context.GetAgents<Ant>().Where(x => x.IsAfterHillVisit))
        {
            var agentHomePheromones = Pheromones.MaxPheromoneValue * Math.Exp(-agent.PathLength / pheromonesPersistence);
            context.SimulationEnvironment.Pheromones[agent.Coordinates.X, agent.Coordinates.Y].AddHome(agentHomePheromones);
        }

        foreach (var hill in context.SimulationEnvironment.AntHills)
        {
            for (var x = hill.Coordinates.X - hill.SizeRadius; x <= hill.Coordinates.X + hill.SizeRadius; x++)
            {
                for (var y = hill.Coordinates.Y - hill.SizeRadius; y <= hill.Coordinates.Y + hill.SizeRadius; y++)
                {
                    if (hill.Coordinates.IsInRange(x, y, hill.SizeRadius))
                    {
                        context.SimulationEnvironment.Pheromones[(int)x, (int)y].AddHome(Pheromones.MaxPheromoneValue);
                    }
                }
            }
        }
    }
}

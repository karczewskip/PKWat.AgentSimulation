namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony;
using System;
using System.Linq;
using System.Threading.Tasks;

public class AddHomePheromones : ISimulationStage
{
    private double pheromonesPersistence = 8;

    public void SetPheromonesPersistence(double pheromonesPersistence)
    {
        this.pheromonesPersistence = pheromonesPersistence;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<ColonyEnvironment>();

        foreach (var agent in context.GetAgents<Ant>().Where(x => x.IsAfterHillVisit))
        {
            var agentHomePheromones = Pheromones.MaxPheromoneValue * Math.Exp(-agent.PathLength / pheromonesPersistence);
            environment.Pheromones[agent.Coordinates.X, agent.Coordinates.Y].AddHome(agentHomePheromones);
        }

        foreach (var hill in environment.AntHills)
        {
            for (var x = hill.Coordinates.X - hill.SizeRadius; x <= hill.Coordinates.X + hill.SizeRadius; x++)
            {
                for (var y = hill.Coordinates.Y - hill.SizeRadius; y <= hill.Coordinates.Y + hill.SizeRadius; y++)
                {
                    if (hill.Coordinates.IsInRange(x, y, hill.SizeRadius))
                    {
                        environment.Pheromones[(int)x, (int)y].AddHome(Pheromones.MaxPheromoneValue);
                    }
                }
            }
        }
    }
}

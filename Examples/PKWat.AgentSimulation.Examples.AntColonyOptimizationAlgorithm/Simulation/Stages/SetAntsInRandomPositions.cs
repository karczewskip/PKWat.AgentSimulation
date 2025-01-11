namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
using System.Threading.Tasks;

internal class SetAntsInRandomPositions(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage<ColonyEnvironment>
{
    public async Task Execute(ISimulationContext<ColonyEnvironment> context)
    {
        foreach(var ant in context.GetAgents<Ant>())
        {
            ant.Coordinates.SetCoordinates(
                1 + randomNumbersGenerator.Next(context.SimulationEnvironment.Width - 2), 
                1 + randomNumbersGenerator.Next(context.SimulationEnvironment.Height - 2));
        }
    }
}

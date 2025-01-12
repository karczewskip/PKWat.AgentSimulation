namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony;
using System.Threading.Tasks;

internal class SetAntsInRandomPositions(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<ColonyEnvironment>();

        foreach (var ant in context.GetAgents<Ant>())
        {
            ant.Coordinates.SetCoordinates(
                1 + randomNumbersGenerator.Next(environment.Width - 2),
                1 + randomNumbersGenerator.Next(environment.Height - 2));
        }
    }
}

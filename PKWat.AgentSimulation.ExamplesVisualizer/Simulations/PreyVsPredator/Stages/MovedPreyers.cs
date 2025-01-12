namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator.Agents;
using System.Linq;
using System.Threading.Tasks;

internal class MovedPreyers(IRandomNumbersGenerator randomNumbersGenerator, ISimulationCyclePerformanceInfo simulationCyclePerformanceInfo) : ISimulationStage
{
    private readonly MovingDirection[] possibleDirections = [MovingDirection.Up, MovingDirection.Down, MovingDirection.Left, MovingDirection.Right];

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<PreyVsPredatorEnvironment>();

        using var step = simulationCyclePerformanceInfo.AddStep("MovedPreyers");
        environment.MovePreys(context.GetAgents<Prey>().Select(x => (x.Id, possibleDirections[randomNumbersGenerator.Next(possibleDirections.Length)])));
    }
}

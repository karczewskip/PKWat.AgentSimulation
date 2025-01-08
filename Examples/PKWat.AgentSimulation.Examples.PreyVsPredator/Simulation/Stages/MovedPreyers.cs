namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;
using System.Linq;
using System.Threading.Tasks;

internal class MovedPreyers(IRandomNumbersGenerator randomNumbersGenerator, ISimulationCyclePerformanceInfo simulationCyclePerformanceInfo) : ISimulationStage<PreyVsPredatorEnvironment>
{
    private readonly MovingDirection[] possibleDirections = [MovingDirection.Up, MovingDirection.Down, MovingDirection.Left, MovingDirection.Right];

    public async Task Execute(ISimulationContext<PreyVsPredatorEnvironment> context)
    {
        using var step = simulationCyclePerformanceInfo.AddStep("MovedPreyers");
        context.SimulationEnvironment.MovePreys(context.GetAgents<Prey>().Select(x => (x.Id, possibleDirections[randomNumbersGenerator.Next(possibleDirections.Length)])));
    }
}

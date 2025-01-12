namespace PKWat.AgentSimulation.Examples.ButterflyEffect.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.ButterflyEffect.Simulation.Agents;
using System.Threading.Tasks;

internal class MoveBallsSlightly : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var allBalls = context.GetAgents<BouncingBall>().ToArray();
        var allBallsNumber = allBalls.Length;

        for(var i = 0; i < allBallsNumber; i++)
        {
            var ball = allBalls[i];
            var deltaX = ((allBallsNumber / 2 - i) * 0.00001) / allBallsNumber;

            ball.Position.Move(deltaX, 0);
        }
    }
}

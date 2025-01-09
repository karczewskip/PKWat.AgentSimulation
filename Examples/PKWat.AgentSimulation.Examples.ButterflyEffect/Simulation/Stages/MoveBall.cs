namespace PKWat.AgentSimulation.Examples.ButterflyEffect.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.ButterflyEffect.Simulation;
using PKWat.AgentSimulation.Examples.ButterflyEffect.Simulation.Agents;
using System.Threading.Tasks;

internal class MoveBall : ISimulationStage<BouncingBallBulb>
{
    public async Task Execute(ISimulationContext<BouncingBallBulb> context)
    {
        Parallel.ForEach(context.GetAgents<BouncingBall>(), new ParallelOptions() { MaxDegreeOfParallelism = 32 }, ball =>
        {
            UpdateBall(
                ball,
                context.SimulationEnvironment.BallRadius,
                context.SimulationEnvironment.BulbRadius,
                context.SimulationEnvironment.Gravity);
        });
    }

    private void UpdateBall(BouncingBall ball, double ballRadius, double bulbRadius, double gravity)
    {
        ball.Position.Move(ball.Velocity);
        var distanceExceeded = ball.Position.DistanceFromCenter + ballRadius - bulbRadius;

        if (distanceExceeded > 0)
        {
            double newDirectionRadian = 2 * ball.Position.NormalRadian - ball.Velocity.Radian;

            var scale = distanceExceeded / ball.Velocity.Speed;
            ball.Position.Move(ball.Velocity, -scale);
            ball.Velocity.ChangeDirection(newDirectionRadian);
            ball.Position.Move(ball.Velocity, 1 - scale);
        }
        else
        {
            ball.Velocity.Accelerate(0, gravity);
        }
    }
}

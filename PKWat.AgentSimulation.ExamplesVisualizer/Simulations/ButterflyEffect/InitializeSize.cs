namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.ButterflyEffect.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.ButterflyEffect;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.ButterflyEffect.Agents;
using System.Threading.Tasks;

internal class InitializeSize(PictureRenderer pictureRenderer) : ISimulationStage
{
    public double BulbRadius { get; private set; } = 400;
    public double BallRadius { get; set; } = 10;
    public double Gravity { get; set; } = 0.25;

    internal void UseSize(double buldRadius, double ballRadius, double gravity)
    {
        BulbRadius = buldRadius;
        BallRadius = ballRadius;
        Gravity = gravity;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<BouncingBallBulb>();

        environment.UseSize(BulbRadius, BallRadius, Gravity);

        var bitmapSize = (int)BulbRadius * 2;

        pictureRenderer.Initialize(
            bitmapSize,
            bitmapSize,
            context.GetAgents<BouncingBall>().Select(x => x.Id).ToArray());
    }
}

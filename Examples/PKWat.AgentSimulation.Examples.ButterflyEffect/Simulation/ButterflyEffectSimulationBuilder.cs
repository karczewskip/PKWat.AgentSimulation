namespace PKWat.AgentSimulation.Examples.ButterflyEffect.Simulation;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.ButterflyEffect.Simulation.Agents;
using PKWat.AgentSimulation.Examples.ButterflyEffect.Simulation.Stages;
using System;
using System.Windows.Media.Imaging;

public class ButterflyEffectSimulationBuilder(ISimulationBuilder simulationBuilder, PictureRenderer pictureRenderer)
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        var simulation = simulationBuilder
            .CreateNewSimulation<BouncingBallBulb>()
            .AddInitializationStage<InitializeSize>(s => s.UseSize(400, 10, 0.25))
            .AddInitializationStage<MoveBallsSlightly>()
            .AddStage<MoveBall>()
            .AddAgents<BouncingBall>(100)
            .AddCallback(c => drawing(pictureRenderer.Draw(c)))
            .SetRandomSeed(100)
            .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(10))
            .Build();
        return simulation;
    }
}

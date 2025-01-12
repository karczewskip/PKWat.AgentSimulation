namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.ButterflyEffect;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.ButterflyEffect.Agents;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.ButterflyEffect.Stages;
using System;
using System.Windows.Media.Imaging;

public class ButterflyEffectSimulationBuilder(ISimulationBuilder simulationBuilder, PictureRenderer pictureRenderer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        var simulation = simulationBuilder
            .CreateNewSimulation<BouncingBallBulb>()
            .AddInitializationStage<InitializeSize>(s => s.UseSize(400, 10, 0.25))
            .AddInitializationStage<MoveBallsSlightly>()
            .AddStage<MoveBall>()
            .AddAgents<BouncingBall>(20_000)
            .AddCallback(c => drawing(pictureRenderer.Draw(c)))
            .Build();
        return simulation;
    }
}

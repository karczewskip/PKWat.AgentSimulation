using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.GameOfLife.Stages;
using System.Windows.Media.Imaging;

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.GameOfLife;

public class GameOfLifeSimulationBuilder(ISimulationBuilder simulationBuilder, GameOfLifeDrawer drawer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        var simulation = simulationBuilder
            .CreateNewSimulation<LifeMatrixEnvironment>()
            .AddInitializationStage<InitializeSize>(s => s.UseSize(500, 500))
            .AddStage<UpdateMatrix>(s => s.UseNumberOfThreads(32))
            .AddCallback(c => drawing(drawer.Draw(c)))
            .SetRandomSeed(100)
            .Build();

        return simulation;
    }
}

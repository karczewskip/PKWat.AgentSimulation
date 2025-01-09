using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.GameOfLife.GPU.Simulation.Stages;
using System.Windows.Media.Imaging;

namespace PKWat.AgentSimulation.Examples.GameOfLife.GPU.Simulation;

public class GameOfLifeSimulationBuilder(ISimulationBuilder simulationBuilder, GameOfLifeDrawer drawer)
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        var simulation = simulationBuilder
            .CreateNewSimulation<LifeMatrixEnvironment>()
            .AddInitializationStage<InitializeSize>(s => s.UseSize(2000, 2000))
            .AddStage<UpdateMatrix>(s => s.UseNumberOfThreads(32))
            .AddCallback(c => drawing(drawer.Draw(c)))
            .SetRandomSeed(100)
            .Build();

        return simulation;
    }
}

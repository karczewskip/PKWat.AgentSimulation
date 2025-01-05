using PKWat.AgentSimulation.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PKWat.AgentSimulation.Examples.GameOfLife.GPU.Simulation;

public class GameOfLifeSimulationBuilder(ISimulationBuilder simulationBuilder, GameOfLifeDrawer drawer)
{
    public ISimulation Build(Action<BitmapSource> drawing, int width, int height)
    {
        var simulation = simulationBuilder
            .CreateNewSimulation<LifeMatrixEnvironment, LifeMatrixEnvironmentState>(
                new LifeMatrixEnvironmentState(new bool[0, 0], 0))
            .AddEnvironmentInitialization(async c => c.SimulationEnvironment.Initialize(width, height))
            .AddEnvironmentUpdates(c => UpdateMatrix(c))
            .AddCallback(c => drawing(drawer.Draw(c)))
            .SetRandomSeed(100)
            .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(1))
            .Build();

        return simulation;
    }

    private async Task UpdateMatrix(ISimulationContext<LifeMatrixEnvironment> context)
        => context.SimulationEnvironment.Update();
}

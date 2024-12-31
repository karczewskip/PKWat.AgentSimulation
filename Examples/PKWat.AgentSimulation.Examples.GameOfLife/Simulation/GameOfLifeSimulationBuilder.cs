using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Examples.GameOfLife.Simulation.Agents;
using System.Windows.Media.Imaging;

namespace PKWat.AgentSimulation.Examples.GameOfLife.Simulation
{
    public class GameOfLifeSimulationBuilder(ISimulationBuilder simulationBuilder, GameOfLifeDrawer drawer)
    {
        public ISimulation Build(Action<BitmapSource> drawing, int width, int height)
        {
            var simulation = simulationBuilder
                .CreateNewSimulation<LifeMatrixEnvironment, LifeMatrixEnvironmentState>(
                    new LifeMatrixEnvironmentState(new Dictionary<AgentId, (int X, int Y)>(), new bool[width, height], 0))
                .AddAgents<Cell>(width * height)
                .AddEnvironmentInitialization(async c => c.SimulationEnvironment.PlaceAgents(c.GetAgents<Cell>().Select(x => x.Id).ToArray()))
                .AddEnvironmentUpdates(c => UpdateMatrix(c))
                .AddCallback(c => drawing(drawer.Draw(c)))
                .SetSimulationStep(TimeSpan.FromMilliseconds(100))
                .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(100))
                .SetRandomSeed(100)
                .Build();

            return simulation;
        }

        private async Task UpdateMatrix(ISimulationContext<LifeMatrixEnvironment> context)
            => context.SimulationEnvironment
                .SetNewMatrix(
                    context.GetAgents<Cell>()
                    .Select(x => new LifeMatrixCellState(x.Id, x.State.IsAlive))
                    .ToArray());
    }
}

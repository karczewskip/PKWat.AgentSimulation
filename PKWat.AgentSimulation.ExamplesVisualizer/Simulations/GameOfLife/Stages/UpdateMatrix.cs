namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.GameOfLife.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.GameOfLife;
using System.Threading.Tasks;

internal class UpdateMatrix : ISimulationStage
{
    private int _numberOfThreads = 2;

    public void UseNumberOfThreads(int numberOfThreads)
    {
        _numberOfThreads = numberOfThreads;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<LifeMatrixEnvironment>();

        environment.Update(_numberOfThreads);
    }
}

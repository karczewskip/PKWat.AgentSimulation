namespace PKWat.AgentSimulation.Examples.GameOfLife.GPU.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using System.Threading.Tasks;

internal class UpdateMatrix : ISimulationStage<LifeMatrixEnvironment>
{
    private int _numberOfThreads = 2;

    public void UseNumberOfThreads(int numberOfThreads)
    {
        _numberOfThreads = numberOfThreads;
    }

    public async Task Execute(ISimulationContext<LifeMatrixEnvironment> context)
    {
        context.SimulationEnvironment.Update(_numberOfThreads);
    }
}

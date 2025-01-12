namespace PKWat.AgentSimulation.Examples.GameOfLife.GPU.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using System.Threading.Tasks;

internal class InitializeSize : ISimulationStage
{
    private int width = 100;
    private int height = 100;

    public void UseSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<LifeMatrixEnvironment>();

        environment.Initialize(width, height);
    }
}

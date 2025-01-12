namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
using System.Threading.Tasks;

internal class SetColonySize(ColonyDrawer colonyDrawer) : ISimulationStage
{
    private int width = 100;
    private int height = 100;

    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;

        colonyDrawer.Initialize(width, height);
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<ColonyEnvironment>();

        environment.SetSize(width, height);
    }
}

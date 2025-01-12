namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using System.Linq;
using System.Threading.Tasks;

internal class SetLindingLines : ISimulationStage
{
    private int maxLandingLines = 8;

    public void SetMaxLandingLines(int maxLandingLines)
    {
        this.maxLandingLines = maxLandingLines;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<AirportEnvironment>();

        environment.SetLandingLines(Enumerable.Range(1, maxLandingLines).ToArray());
    }
}

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

namespace PKWat.AgentSimulation.Examples.Airport2.Stages;

public class SetLandingLines : ISimulationStage
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

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.Airport2.Agents;

namespace PKWat.AgentSimulation.Examples.Airport2.Stages;

public class ReleaseLines : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        foreach (var airplane in context.GetAgents<Airplane>().Where(
            x => x.IsDepartured(context.Time.Time)
            && x.AssignedLine.HasValue).ToArray())
        {
            var environment = context.GetSimulationEnvironment<AirportEnvironment>();

            environment.AvailableLines.Enqueue(airplane.AssignedLine!.Value);
            airplane.AssignedLine = null;
        }
    }
}

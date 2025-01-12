namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Agents;
using System.Linq;
using System.Threading.Tasks;

internal class ReleaseLines : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        foreach (var airplane in context.GetAgents<Airplane>().Where(
            x => x.IsDepartured(context.SimulationTime.Time)
            && x.AssignedLine.HasValue).ToArray())
        {
            var environment = context.GetSimulationEnvironment<AirportEnvironment>();

            environment.AvailableLines.Enqueue(airplane.AssignedLine!.Value);
            airplane.AssignedLine = null;
        }
    }
}

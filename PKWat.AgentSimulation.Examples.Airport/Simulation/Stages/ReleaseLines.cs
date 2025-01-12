namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;
using System.Linq;
using System.Threading.Tasks;

internal class ReleaseLines : ISimulationStage<AirportEnvironment>
{
    public async Task Execute(ISimulationContext<AirportEnvironment> context)
    {
        foreach(var airplane in context.GetAgents<Airplane>().Where(
            x => x.IsDepartured(context.SimulationTime.Time) 
            && x.AssignedLine.HasValue).ToArray())
        {
            context.SimulationEnvironment.AvailableLines.Enqueue(airplane.AssignedLine!.Value);
            airplane.AssignedLine = null;
        }
    }
}

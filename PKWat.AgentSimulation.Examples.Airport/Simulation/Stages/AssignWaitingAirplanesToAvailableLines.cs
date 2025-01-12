namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;
using System.Linq;
using System.Threading.Tasks;

internal class AssignWaitingAirplanesToAvailableLines : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<AirportEnvironment>();

        while (environment.WaitingAirplanes.Any() && environment.AvailableLines.Any())
        {
            var airplaneId = environment.WaitingAirplanes.Dequeue();
            var line = environment.AvailableLines.Dequeue();
            var airplane = context.GetRequiredAgent<Airplane>(airplaneId);
            airplane.AssignedLine = line;
        }
    }
}

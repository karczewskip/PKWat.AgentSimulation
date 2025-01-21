namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Agents;
using System.Linq;
using System.Threading.Tasks;

public class AssignWaitingAirplanesToAvailableLines : ISimulationStage
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

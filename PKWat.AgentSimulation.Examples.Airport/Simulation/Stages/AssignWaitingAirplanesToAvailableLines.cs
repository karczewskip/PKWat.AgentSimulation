namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;
using System.Linq;
using System.Threading.Tasks;

internal class AssignWaitingAirplanesToAvailableLines : ISimulationStage<AirportEnvironment>
{
    public async Task Execute(ISimulationContext<AirportEnvironment> context)
    {
        while(context.SimulationEnvironment.WaitingAirplanes.Any() && context.SimulationEnvironment.AvailableLines.Any())
        {
            var airplaneId = context.SimulationEnvironment.WaitingAirplanes.Dequeue();
            var line = context.SimulationEnvironment.AvailableLines.Dequeue();
            var airplane = context.GetRequiredAgent<Airplane>(airplaneId);
            airplane.AssignedLine = line;
        }
    }
}

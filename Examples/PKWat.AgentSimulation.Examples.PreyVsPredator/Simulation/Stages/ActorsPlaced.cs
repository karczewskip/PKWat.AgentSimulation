namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;
using System.Linq;
using System.Threading.Tasks;

internal class ActorsPlaced : ISimulationStage<PreyVsPredatorEnvironment>
{
    public async Task Execute(ISimulationContext<PreyVsPredatorEnvironment> context)
    {
        context.SimulationEnvironment.PlaceInitialPreys(context.GetAgents<Prey>().Select(x => x.Id).ToArray());
        context.SimulationEnvironment.PlaceInitialPredators(context.GetAgents<Predator>().Select(x => x.Id).ToArray());
    }
}

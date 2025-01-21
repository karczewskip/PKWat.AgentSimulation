namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator.Agents;
using System.Linq;
using System.Threading.Tasks;

public class ActorsPlaced : ISimulationStage
{
    private int width;
    private int height;

    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<PreyVsPredatorEnvironment>();

        environment.SetSize(width, height);

        environment.PlaceInitialPreys(context.GetAgents<Prey>().Select(x => x.Id).ToArray());
        environment.PlaceInitialPredators(context.GetAgents<Predator>().Select(x => x.Id).ToArray());
    }
}

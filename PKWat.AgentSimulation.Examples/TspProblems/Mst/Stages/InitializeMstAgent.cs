namespace PKWat.AgentSimulation.Examples.TspProblems.Mst.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.TspProblems.Agents;
using System.Threading.Tasks;

public class InitializeMstAgent : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspEnvironment>();
        var agent = context.AddAgent<MstAgent>();

        if (environment.DistanceMatrix != null)
        {
            agent.Initialize(environment.Points.Count, environment.DistanceMatrix);
        }

        await Task.CompletedTask;
    }
}

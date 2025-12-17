namespace PKWat.AgentSimulation.Examples.TspProblems.BruteForce.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.TspProblems.Agents;
using System.Threading.Tasks;

public class InitializeBruteForceAgent : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspEnvironment>();
        var agent = context.AddAgent<BruteForceAgent>();

        agent.Initialize(environment.Points.Count);

        await Task.CompletedTask;
    }
}

namespace PKWat.AgentSimulation.Examples.TspProblems.HeldKarp.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.TspProblems.Agents;
using System.Threading.Tasks;

public class InitializeHeldKarpAgent : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspEnvironment>();
        var agent = context.AddAgent<HeldKarpAgent>();

        agent.Initialize(environment.Points.Count);

        await Task.CompletedTask;
    }
}

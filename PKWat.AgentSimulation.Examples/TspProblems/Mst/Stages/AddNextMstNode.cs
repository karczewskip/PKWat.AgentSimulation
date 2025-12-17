namespace PKWat.AgentSimulation.Examples.TspProblems.Mst.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.TspProblems.Agents;
using System.Threading.Tasks;

public class AddNextMstNode : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspEnvironment>();
        var agent = context.GetAgents<MstAgent>().First();

        if (!agent.IsMstBuilt || agent.IsComplete || agent.MstRoute == null)
            return;

        // Add one node from MST route to current route
        if (agent.CurrentStep < agent.MstRoute.Count)
        {
            int nextNode = agent.MstRoute[agent.CurrentStep];
            agent.AddNodeToRoute(nextNode);

            // Calculate current route distance
            if (agent.CurrentRoute!.Count >= 2)
            {
                double distance = environment.CalculateRouteDistance(agent.CurrentRoute);
                var solution = TspSolution.Create(agent.CurrentRoute, distance);
                agent.SetBestSolution(solution);
                environment.UpdateBestSolution(solution);
            }
        }

        // Mark complete when all nodes are added
        if (agent.CurrentStep >= agent.MstRoute.Count)
        {
            agent.MarkComplete();
        }

        await Task.CompletedTask;
    }
}

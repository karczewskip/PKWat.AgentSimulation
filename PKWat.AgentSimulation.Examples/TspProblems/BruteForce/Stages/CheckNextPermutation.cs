namespace PKWat.AgentSimulation.Examples.TspProblems.BruteForce.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.TspProblems.Agents;
using System.Threading.Tasks;

public class CheckNextPermutation : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspEnvironment>();
        var agent = context.GetAgents<BruteForceAgent>().First();

        if (agent.IsComplete || agent.CurrentPermutation == null)
            return;

        // Evaluate current permutation
        double distance = environment.CalculateRouteDistance(agent.CurrentPermutation);
        var solution = TspSolution.Create(agent.CurrentPermutation, distance);
        agent.SetCurrentSolution(solution);
        environment.UpdateBestSolution(solution);

        // Generate next permutation
        var nextPermutation = new List<int>(agent.CurrentPermutation);
        if (!NextPermutation(nextPermutation))
        {
            agent.MarkComplete();
        }
        else
        {
            agent.UpdatePermutation(nextPermutation);
        }

        await Task.CompletedTask;
    }

    private bool NextPermutation(List<int> array)
    {
        // Find the largest index i such that array[i] < array[i + 1]
        int i = array.Count - 2;
        while (i >= 0 && array[i] >= array[i + 1])
            i--;

        if (i < 0)
            return false; // No more permutations

        // Find the largest index j such that array[i] < array[j]
        int j = array.Count - 1;
        while (array[i] >= array[j])
            j--;

        // Swap array[i] and array[j]
        (array[i], array[j]) = (array[j], array[i]);

        // Reverse the suffix starting at array[i + 1]
        array.Reverse(i + 1, array.Count - i - 1);

        return true;
    }
}

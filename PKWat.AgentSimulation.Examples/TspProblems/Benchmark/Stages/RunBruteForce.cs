namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class RunBruteForce : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspBenchmarkEnvironment>();
        var agent = context.GetAgents<TspBenchmarkAgent>()
            .FirstOrDefault(a => a.AlgorithmType == TspAlgorithmType.BruteForce && !a.IsComplete && !a.HasExceededTimeLimit);

        if (agent == null || environment.CurrentDistanceMatrix == null)
            return;

        if (agent.CheckTimeLimit())
            return;

        int n = environment.CurrentPoints.Count;
        var route = Enumerable.Range(0, n).ToList();
        double bestDistance = double.MaxValue;
        List<int>? bestRoute = null;

        do
        {
            double distance = environment.CalculateRouteDistance(route);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestRoute = new List<int>(route);
            }

            if (agent.CheckTimeLimit())
                return;

        } while (NextPermutation(route));

        if (bestRoute != null)
        {
            agent.SetBestSolution(TspSolution.Create(bestRoute, bestDistance));
        }
        
        agent.MarkComplete();
        await Task.CompletedTask;
    }

    private bool NextPermutation(List<int> array)
    {
        int i = array.Count - 2;
        while (i >= 0 && array[i] >= array[i + 1])
            i--;

        if (i < 0)
            return false;

        int j = array.Count - 1;
        while (array[j] <= array[i])
            j--;

        (array[i], array[j]) = (array[j], array[i]);

        int left = i + 1;
        int right = array.Count - 1;
        while (left < right)
        {
            (array[left], array[right]) = (array[right], array[left]);
            left++;
            right--;
        }

        return true;
    }
}

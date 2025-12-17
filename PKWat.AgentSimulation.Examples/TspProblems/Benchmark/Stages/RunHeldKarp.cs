namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class RunHeldKarp : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspBenchmarkEnvironment>();
        var agent = context.GetAgents<TspBenchmarkAgent>()
            .FirstOrDefault(a => a.AlgorithmType == TspAlgorithmType.HeldKarp && !a.IsComplete && !a.HasExceededTimeLimit);

        if (agent == null || environment.CurrentDistanceMatrix == null)
            return;

        // Start timing for this test case
        agent.StartNewRound(environment.CurrentPointCount, environment.CurrentExampleIndex);

        if (agent.CheckTimeLimit())
            return;

        int n = environment.CurrentPoints.Count;
        var distances = environment.CurrentDistanceMatrix;
        
        var dp = new Dictionary<(int, int), double>();
        var parent = new Dictionary<(int, int), int>();

        int startMask = 1 << 0;
        dp[(startMask, 0)] = 0;

        for (int size = 1; size < n; size++)
        {
            var newDp = new Dictionary<(int, int), double>();
            
            foreach (var ((mask, last), dist) in dp)
            {
                if (agent.CheckTimeLimit())
                    return;

                for (int next = 0; next < n; next++)
                {
                    if ((mask & (1 << next)) != 0)
                        continue;

                    int newMask = mask | (1 << next);
                    double newDist = dist + distances[last, next];
                    var key = (newMask, next);

                    if (!newDp.ContainsKey(key) || newDist < newDp[key])
                    {
                        newDp[key] = newDist;
                        parent[key] = last;
                    }
                }
            }
            
            dp = newDp;
        }

        int fullMask = (1 << n) - 1;
        double minDist = double.MaxValue;
        int lastNode = -1;

        for (int i = 1; i < n; i++)
        {
            var key = (fullMask, i);
            if (dp.ContainsKey(key))
            {
                double totalDist = dp[key] + distances[i, 0];
                if (totalDist < minDist)
                {
                    minDist = totalDist;
                    lastNode = i;
                }
            }
        }

        if (lastNode != -1)
        {
            var route = new List<int>();
            int currentMask = fullMask;
            int currentNode = lastNode;

            while (currentMask != startMask)
            {
                route.Add(currentNode);
                var key = (currentMask, currentNode);
                int prevNode = parent[key];
                currentMask ^= (1 << currentNode);
                currentNode = prevNode;
            }

            route.Add(0);
            route.Reverse();

            agent.SetBestSolution(TspSolution.Create(route, minDist));
        }

        agent.MarkComplete();
        await Task.CompletedTask;
    }
}

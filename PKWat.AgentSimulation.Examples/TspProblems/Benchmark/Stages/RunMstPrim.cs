namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class RunMstPrim : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspBenchmarkEnvironment>();
        var agent = context.GetAgents<TspBenchmarkAgent>()
            .FirstOrDefault(a => a.AlgorithmType == TspAlgorithmType.MstPrim && !a.IsComplete && !a.HasExceededTimeLimit);

        if (agent == null || environment.CurrentDistanceMatrix == null)
            return;

        if (agent.CheckTimeLimit())
            return;

        int n = environment.CurrentPoints.Count;
        var distances = environment.CurrentDistanceMatrix;
        
        var inMst = new bool[n];
        var parent = new int[n];
        var key = new double[n];

        for (int i = 0; i < n; i++)
        {
            key[i] = double.MaxValue;
            parent[i] = -1;
        }

        key[0] = 0;
        var mstEdges = new List<(int from, int to)>();

        for (int count = 0; count < n; count++)
        {
            if (agent.CheckTimeLimit())
                return;

            int u = -1;
            double minKey = double.MaxValue;

            for (int v = 0; v < n; v++)
            {
                if (!inMst[v] && key[v] < minKey)
                {
                    minKey = key[v];
                    u = v;
                }
            }

            if (u == -1)
                break;

            inMst[u] = true;

            if (parent[u] != -1)
            {
                mstEdges.Add((parent[u], u));
            }

            for (int v = 0; v < n; v++)
            {
                if (!inMst[v] && distances[u, v] < key[v])
                {
                    parent[v] = u;
                    key[v] = distances[u, v];
                }
            }
        }

        var route = ComputeDfsRoute(mstEdges, n);
        double totalDistance = environment.CalculateRouteDistance(route);

        agent.SetBestSolution(TspSolution.Create(route, totalDistance));
        agent.MarkComplete();

        await Task.CompletedTask;
    }

    private List<int> ComputeDfsRoute(List<(int from, int to)> edges, int n)
    {
        var adj = new List<int>[n];
        for (int i = 0; i < n; i++)
            adj[i] = new List<int>();

        foreach (var (from, to) in edges)
        {
            adj[from].Add(to);
            adj[to].Add(from);
        }

        var route = new List<int>();
        var visited = new HashSet<int>();

        void Dfs(int node)
        {
            visited.Add(node);
            route.Add(node);

            foreach (var neighbor in adj[node])
            {
                if (!visited.Contains(neighbor))
                {
                    Dfs(neighbor);
                }
            }
        }

        Dfs(0);
        return route;
    }
}

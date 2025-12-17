namespace PKWat.AgentSimulation.Examples.TspProblems.Mst.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.TspProblems.Agents;
using System.Threading.Tasks;

public class BuildMstWithPrim : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspEnvironment>();
        var agent = context.GetAgents<MstAgent>().First();

        if (agent.IsMstBuilt || environment.DistanceMatrix == null)
            return;

        int n = environment.Points.Count;
        var distances = environment.DistanceMatrix;

        // Prim's algorithm to build MST
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
            // Find minimum key vertex not yet in MST
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

            if (u == -1) break;

            inMst[u] = true;

            if (parent[u] != -1)
            {
                mstEdges.Add((parent[u], u));
            }

            // Update key values of adjacent vertices
            for (int v = 0; v < n; v++)
            {
                if (!inMst[v] && distances[u, v] < key[v])
                {
                    parent[v] = u;
                    key[v] = distances[u, v];
                }
            }
        }

        // Convert MST to DFS order for TSP approximation
        var mstRoute = DfsTraversal(mstEdges, n);
        agent.BuildMst(mstRoute);

        await Task.CompletedTask;
    }

    private List<int> DfsTraversal(List<(int from, int to)> edges, int n)
    {
        // Build adjacency list
        var adj = new List<int>[n];
        for (int i = 0; i < n; i++)
            adj[i] = new List<int>();

        foreach (var (from, to) in edges)
        {
            adj[from].Add(to);
            adj[to].Add(from);
        }

        // DFS traversal
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

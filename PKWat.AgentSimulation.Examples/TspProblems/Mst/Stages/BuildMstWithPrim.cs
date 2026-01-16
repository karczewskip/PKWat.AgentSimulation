namespace PKWat.AgentSimulation.Examples.TspProblems.Mst.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.TspProblems.Agents;
using System.Threading.Tasks;

public class BuildMstWithPrim : ISimulationStage
{
    private bool[]? _inMst;
    private int[]? _parent;
    private double[]? _key;
    private int _nodesAdded = 0;
    private int _totalNodes = 0;

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspEnvironment>();
        var agent = context.GetAgents<MstAgent>().First();

        if (agent.IsMstBuilt || environment.DistanceMatrix == null)
            return;

        int n = environment.Points.Count;
        var distances = environment.DistanceMatrix;

        // Initialize Prim's algorithm on first call
        if (_inMst == null)
        {
            _totalNodes = n;
            _inMst = new bool[n];
            _parent = new int[n];
            _key = new double[n];

            for (int i = 0; i < n; i++)
            {
                _key[i] = double.MaxValue;
                _parent[i] = -1;
            }

            _key[0] = 0;
            _nodesAdded = 0;
        }

        // Add one node to MST per step
        if (_nodesAdded < _totalNodes)
        {
            // Find minimum key vertex not yet in MST
            int u = -1;
            double minKey = double.MaxValue;
            
            for (int v = 0; v < n; v++)
            {
                if (!_inMst[v] && _key[v] < minKey)
                {
                    minKey = _key[v];
                    u = v;
                }
            }

            if (u == -1) return;

            _inMst[u] = true;

            // Add edge to MST (except for the first node)
            if (_parent[u] != -1)
            {
                agent.AddMstEdge(_parent[u], u);
            }

            _nodesAdded++;

            // Update key values of adjacent vertices
            for (int v = 0; v < n; v++)
            {
                if (!_inMst[v] && distances[u, v] < _key[v])
                {
                    _parent[v] = u;
                    _key[v] = distances[u, v];
                }
            }

            // If all nodes added, compute DFS route and mark MST as built
            if (_nodesAdded >= _totalNodes)
            {
                var dfsRoute = ComputeDfsRoute(agent.MstEdges!, n);
                agent.CompleteMstBuilding(dfsRoute);
            }
        }

        await Task.CompletedTask;
    }

    private List<int> ComputeDfsRoute(List<(int from, int to)> edges, int n)
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

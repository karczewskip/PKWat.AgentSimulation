namespace PKWat.AgentSimulation.Math.Algorithms.TSP;

public class MstPrimAlgorithm : ITspAlgorithm
{
    public TspSolution? Solve(List<TspPoint> points, CancellationToken cancellationToken)
    {
        if (points.Count < 2)
            return TspSolution.Empty();

        int n = points.Count;
        var distanceMatrix = BuildDistanceMatrix(points);
        
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
            if (cancellationToken.IsCancellationRequested)
                return null;

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
                if (!inMst[v] && distanceMatrix[u, v] < key[v])
                {
                    parent[v] = u;
                    key[v] = distanceMatrix[u, v];
                }
            }
        }

        var route = ComputeDfsRoute(mstEdges, n);
        double totalDistance = CalculateRouteDistance(route, distanceMatrix);

        return TspSolution.Create(route, totalDistance);
    }

    private double[,] BuildDistanceMatrix(List<TspPoint> points)
    {
        int n = points.Count;
        var matrix = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] = points[i].DistanceTo(points[j]);
            }
        }

        return matrix;
    }

    private double CalculateRouteDistance(List<int> route, double[,] distanceMatrix)
    {
        if (route.Count < 2)
            return 0;

        double distance = 0;
        for (int i = 0; i < route.Count - 1; i++)
        {
            distance += distanceMatrix[route[i], route[i + 1]];
        }
        distance += distanceMatrix[route[^1], route[0]];
        
        return distance;
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

namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark.Algorithms;

public class HeldKarpAlgorithm : ITspAlgorithm
{
    public TspSolution? Solve(List<TspPoint> points, CancellationToken cancellationToken)
    {
        if (points.Count < 2)
            return TspSolution.Empty();

        int n = points.Count;
        var distanceMatrix = BuildDistanceMatrix(points);
        
        var dp = new Dictionary<(int, int), double>();
        var parent = new Dictionary<(int, int), int>();

        int startMask = 1 << 0;
        dp[(startMask, 0)] = 0;

        for (int size = 1; size < n; size++)
        {
            var newDp = new Dictionary<(int, int), double>();
            
            foreach (var ((mask, last), dist) in dp)
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;

                for (int next = 0; next < n; next++)
                {
                    if ((mask & (1 << next)) != 0)
                        continue;

                    int newMask = mask | (1 << next);
                    double newDist = dist + distanceMatrix[last, next];
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
                double totalDist = dp[key] + distanceMatrix[i, 0];
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

            return TspSolution.Create(route, minDist);
        }

        return TspSolution.Empty();
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
}

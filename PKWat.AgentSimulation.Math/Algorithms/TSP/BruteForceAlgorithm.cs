namespace PKWat.AgentSimulation.Math.Algorithms.TSP;

public class BruteForceAlgorithm : ITspAlgorithm
{
    public TspSolution? Solve(List<TspPoint> points, CancellationToken cancellationToken)
    {
        if (points.Count < 2)
            return TspSolution.Empty();

        int n = points.Count;
        var distanceMatrix = BuildDistanceMatrix(points);
        
        var route = Enumerable.Range(0, n).ToList();
        double bestDistance = double.MaxValue;
        List<int>? bestRoute = null;

        do
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            double distance = CalculateRouteDistance(route, distanceMatrix);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestRoute = new List<int>(route);
            }

        } while (NextPermutation(route));

        if (bestRoute != null)
        {
            return TspSolution.Create(bestRoute, bestDistance);
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

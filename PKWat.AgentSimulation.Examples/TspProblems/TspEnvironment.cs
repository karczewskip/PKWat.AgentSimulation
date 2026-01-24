namespace PKWat.AgentSimulation.Examples.TspProblems;

using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.SimMath.Algorithms.TSP;

public class TspEnvironment : DefaultSimulationEnvironment
{
    private readonly List<TspPoint> _points = new();
    private TspSolution? _bestSolution = null;
    private double _searchSpaceWidth = 100.0;
    private double _searchSpaceHeight = 100.0;
    private double[,]? _distanceMatrix;

    public IReadOnlyList<TspPoint> Points => _points.AsReadOnly();
    public TspSolution? BestSolution => _bestSolution;
    public double SearchSpaceWidth => _searchSpaceWidth;
    public double SearchSpaceHeight => _searchSpaceHeight;
    public double[,]? DistanceMatrix => _distanceMatrix;

    public void SetSearchSpaceSize(double width, double height)
    {
        _searchSpaceWidth = width;
        _searchSpaceHeight = height;
    }

    public void AddPoint(TspPoint point)
    {
        _points.Add(point);
    }

    public void BuildDistanceMatrix()
    {
        int n = _points.Count;
        _distanceMatrix = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                _distanceMatrix[i, j] = _points[i].DistanceTo(_points[j]);
            }
        }
    }

    public void UpdateBestSolution(TspSolution solution)
    {
        if (_bestSolution == null || solution.TotalDistance < _bestSolution.TotalDistance)
        {
            _bestSolution = solution;
        }
    }

    public double CalculateRouteDistance(List<int> route)
    {
        if (route.Count < 2 || _distanceMatrix == null)
            return 0;

        double distance = 0;
        for (int i = 0; i < route.Count - 1; i++)
        {
            distance += _distanceMatrix[route[i], route[i + 1]];
        }
        // Return to start
        distance += _distanceMatrix[route[^1], route[0]];
        
        return distance;
    }

    public bool HasFoundOptimalSolution(double threshold)
    {
        return _bestSolution != null && _bestSolution.TotalDistance <= threshold;
    }
}

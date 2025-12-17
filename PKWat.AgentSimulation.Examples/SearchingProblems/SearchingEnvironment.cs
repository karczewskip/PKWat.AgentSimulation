namespace PKWat.AgentSimulation.Examples.SearchingProblems;

using PKWat.AgentSimulation.Core.Environment;
using System.Collections.Generic;

public class SearchingEnvironment : DefaultSimulationEnvironment
{
    private readonly List<SearchPoint> _points = new();
    private SearchPoint? _bestPoint = null;
    private double _searchSpaceWidth = 100.0;
    private double _searchSpaceHeight = 100.0;

    public IReadOnlyList<SearchPoint> Points => _points.AsReadOnly();
    public SearchPoint? BestPoint => _bestPoint;
    public double SearchSpaceWidth => _searchSpaceWidth;
    public double SearchSpaceHeight => _searchSpaceHeight;

    public void SetSearchSpaceSize(double width, double height)
    {
        _searchSpaceWidth = width;
        _searchSpaceHeight = height;
    }

    public void AddPoint(SearchPoint point)
    {
        _points.Add(point);
        UpdateBestPoint(point);
    }

    public void UpdateBestPoint(SearchPoint point)
    {
        if (_bestPoint == null || point.Value > _bestPoint.Value)
        {
            _bestPoint = point;
        }
    }

    public bool HasFoundOptimalSolution(double threshold)
    {
        return _bestPoint != null && _bestPoint.Value >= threshold;
    }

    public void ClearPoints()
    {
        _points.Clear();
    }
}

namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark;

using PKWat.AgentSimulation.Core.Environment;

public class TspBenchmarkEnvironment : DefaultSimulationEnvironment
{
    private readonly List<List<TspPoint>> _testCases = new();
    private int _currentPointCount = 3;
    private int _currentExampleIndex = 0;
    private readonly int _examplesPerRound = 10;
    private int _startingPointCount = 3;
    
    public IReadOnlyList<List<TspPoint>> TestCases => _testCases.AsReadOnly();
    public int CurrentPointCount => _currentPointCount;
    public int CurrentExampleIndex => _currentExampleIndex;
    public int ExamplesPerRound => _examplesPerRound;
    public List<TspPoint> CurrentPoints { get; private set; } = new();
    public double[,]? CurrentDistanceMatrix { get; private set; }

    public void SetStartingPointCount(int startingPointCount)
    {
        _startingPointCount = startingPointCount;
        _currentPointCount = startingPointCount;
    }

    public void GenerateTestCases(int maxPointCount, Random random)
    {
        _testCases.Clear();
        
        for (int pointCount = _startingPointCount; pointCount <= maxPointCount; pointCount++)
        {
            for (int example = 0; example < _examplesPerRound; example++)
            {
                var points = new List<TspPoint>();
                for (int i = 0; i < pointCount; i++)
                {
                    points.Add(TspPoint.Create(i, random.NextDouble() * 100, random.NextDouble() * 100));
                }
                _testCases.Add(points);
            }
        }
    }

    public bool MoveToNextExample()
    {
        _currentExampleIndex++;
        
        if (_currentExampleIndex >= _examplesPerRound)
        {
            _currentExampleIndex = 0;
            _currentPointCount++;
            return true;
        }
        
        return false;
    }

    public void LoadCurrentTestCase()
    {
        int testCaseIndex = (_currentPointCount - _startingPointCount) * _examplesPerRound + _currentExampleIndex;
        
        if (testCaseIndex < _testCases.Count)
        {
            CurrentPoints = new List<TspPoint>(_testCases[testCaseIndex]);
            BuildDistanceMatrix();
        }
    }

    private void BuildDistanceMatrix()
    {
        int n = CurrentPoints.Count;
        CurrentDistanceMatrix = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                CurrentDistanceMatrix[i, j] = CurrentPoints[i].DistanceTo(CurrentPoints[j]);
            }
        }
    }

    public double CalculateRouteDistance(List<int> route)
    {
        if (route.Count < 2 || CurrentDistanceMatrix == null)
            return 0;

        double distance = 0;
        for (int i = 0; i < route.Count - 1; i++)
        {
            distance += CurrentDistanceMatrix[route[i], route[i + 1]];
        }
        distance += CurrentDistanceMatrix[route[^1], route[0]];
        
        return distance;
    }
}

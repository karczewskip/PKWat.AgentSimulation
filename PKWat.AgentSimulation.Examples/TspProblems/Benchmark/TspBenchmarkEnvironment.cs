namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark;

using PKWat.AgentSimulation.Core.Environment;

public class TspBenchmarkEnvironment : DefaultSimulationEnvironment
{
    private readonly Dictionary<int, List<List<TspPoint>>> _testCasesByPointCount = new();
    private int _currentPointCount = 4;
    private int _currentExampleIndex = 0;
    private readonly int _examplesPerRound = 100;
    
    public int CurrentPointCount => _currentPointCount;
    public int CurrentExampleIndex => _currentExampleIndex;
    public int ExamplesPerRound => _examplesPerRound;
    public List<TspPoint> CurrentPoints { get; private set; } = new();
    public double[,]? CurrentDistanceMatrix { get; private set; }

    public void SetStartingPointCount(int startingPointCount)
    {
        _currentPointCount = startingPointCount;
    }

    public void GenerateTestCasesForPointCount(int pointCount, Random random)
    {
        // Skip if already generated
        if (_testCasesByPointCount.ContainsKey(pointCount))
            return;

        var testCases = new List<List<TspPoint>>();
        
        for (int example = 0; example < _examplesPerRound; example++)
        {
            var points = new List<TspPoint>();
            for (int i = 0; i < pointCount; i++)
            {
                points.Add(TspPoint.Create(i, random.NextDouble() * 100, random.NextDouble() * 100));
            }
            testCases.Add(points);
        }
        
        _testCasesByPointCount[pointCount] = testCases;
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
        if (!_testCasesByPointCount.ContainsKey(_currentPointCount))
            return;

        var testCases = _testCasesByPointCount[_currentPointCount];
        
        if (_currentExampleIndex < testCases.Count)
        {
            CurrentPoints = new List<TspPoint>(testCases[_currentExampleIndex]);
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

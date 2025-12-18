namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark;

using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Math.Algorithms.TSP;

public class TspBenchmarkEnvironment : DefaultSimulationEnvironment
{
    private readonly TspTestCaseGenerator _testCaseGenerator;
    private readonly List<TspPoint> _currentTestCase = new();
    private int _currentPointCount = 4;
    private int _currentExampleIndex = 0;
    private readonly int _examplesPerRound = 100;
    private readonly double _maxCoordinate = 100.0;

    public int CurrentPointCount => _currentPointCount;
    public int CurrentExampleIndex => _currentExampleIndex;
    public int ExamplesPerRound => _examplesPerRound;
    public List<TspPoint> CurrentPoints => _currentTestCase;
    public double[,]? CurrentDistanceMatrix { get; private set; }

    public TspBenchmarkEnvironment(IRandomNumbersGenerator randomNumbersGenerator)
    {
        _testCaseGenerator = new TspTestCaseGenerator(randomNumbersGenerator);
    }

    public void SetStartingPointCount(int startingPointCount)
    {
        _currentPointCount = startingPointCount;
    }

    public void GenerateTestCasesForPointCount()
    {
        _currentTestCase.Clear();
        
        var points = _testCaseGenerator.GenerateTestCase(_currentPointCount, _maxCoordinate);
        _currentTestCase.AddRange(points);

        BuildDistanceMatrix();
    }

    public void MoveToNextExample()
    {
        if (_currentTestCase.Any())
        {
            _currentExampleIndex = (_currentExampleIndex + 1) % _examplesPerRound;
            if (_currentExampleIndex == 0)
            {
                _currentPointCount++;
            }
        }

        GenerateTestCasesForPointCount();
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

namespace PKWat.AgentSimulation.Examples.TspProblems.Benchmark;

using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.RandomNumbers;

public class TspBenchmarkEnvironment(IRandomNumbersGenerator randomNumbersGenerator) : DefaultSimulationEnvironment
{
    private readonly List<TspPoint> _currentTestCase = new();
    private int _currentPointCount = 4;
    private int _currentExampleIndex = 0;
    private readonly int _examplesPerRound = 100;
    
    public int CurrentPointCount => _currentPointCount;
    public int CurrentExampleIndex => _currentExampleIndex;
    public int ExamplesPerRound => _examplesPerRound;
    public List<TspPoint> CurrentPoints => _currentTestCase;
    public double[,]? CurrentDistanceMatrix { get; private set; }

    public void SetStartingPointCount(int startingPointCount)
    {
        _currentPointCount = startingPointCount;
    }

    public void GenerateTestCasesForPointCount()
    {
        _currentTestCase.Clear();
        
        for (int i = 0; i < _currentPointCount; i++)
        {
            _currentTestCase.Add(TspPoint.Create(i, randomNumbersGenerator.NextDouble() * 100, randomNumbersGenerator.NextDouble() * 100));
        }

        BuildDistanceMatrix();
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

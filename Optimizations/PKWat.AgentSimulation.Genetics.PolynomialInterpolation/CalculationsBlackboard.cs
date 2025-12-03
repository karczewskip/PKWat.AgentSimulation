using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Environment;
using System.Collections.Concurrent;

namespace PKWat.AgentSimulation.Genetics.PolynomialInterpolation;

public class CalculationsBlackboard : DefaultSimulationEnvironment
{
    public ConcurrentDictionary<AgentId, ErrorResult> AgentErrors { get; } = new();
    public ExpectedValues ExpectedValues { get; private set; }
    public int NumberOfChecksWithoutImprovement { get; set; } = 0;
    public bool ImprovedFromLastCheck => NumberOfChecksWithoutImprovement == 0;
    public double BestErrorSoFar { get; set; } = double.MaxValue;

    public void SetExpectedValues(ExpectedValues expectedValues)
    {
        ExpectedValues = expectedValues;
    }

    internal void CheckBestResult()
    {
        var currentBestError = AgentErrors.Values.Min(x => x.MeanAbsoluteError);
        if (Math.Abs(currentBestError - BestErrorSoFar) > 1e-6)
        {
            BestErrorSoFar = currentBestError;
            NumberOfChecksWithoutImprovement = 0;
        }
        else
        {
            NumberOfChecksWithoutImprovement++;
        }
    }
}

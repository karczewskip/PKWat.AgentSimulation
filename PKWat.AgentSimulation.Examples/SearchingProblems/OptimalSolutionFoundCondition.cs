namespace PKWat.AgentSimulation.Examples.SearchingProblems;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Crash;

public class OptimalSolutionFoundCondition
{
    private readonly double _optimalValueThreshold;
    private readonly long _maxIterations;

    public OptimalSolutionFoundCondition(double optimalValueThreshold = 99.5, long maxIterations = 1000)
    {
        _optimalValueThreshold = optimalValueThreshold;
        _maxIterations = maxIterations;
    }

    public SimulationCrashResult CheckCondition(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<SearchingEnvironment>();

        // Stop if we found the optimal solution
        if (environment.HasFoundOptimalSolution(_optimalValueThreshold))
        {
            return SimulationCrashResult.Crash($"Optimal solution found! Best value: {environment.BestPoint?.Value:F2} at ({environment.BestPoint?.X:F2}, {environment.BestPoint?.Y:F2})");
        }

        // Stop if max iterations reached
        if (context.Time.StepNo >= _maxIterations)
        {
            return SimulationCrashResult.Crash($"Max iterations reached. Best value: {environment.BestPoint?.Value:F2} at ({environment.BestPoint?.X:F2}, {environment.BestPoint?.Y:F2})");
        }

        return SimulationCrashResult.NoCrash;
    }
}

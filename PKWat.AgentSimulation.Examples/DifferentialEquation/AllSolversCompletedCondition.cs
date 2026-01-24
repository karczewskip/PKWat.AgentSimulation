namespace PKWat.AgentSimulation.Examples.DifferentialEquation;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Crash;
using PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

public class AllSolversCompletedCondition
{
    private readonly long _maxIterations;

    public AllSolversCompletedCondition(long maxIterations = 10000)
    {
        _maxIterations = maxIterations;
    }

    public SimulationCrashResult CheckCondition(ISimulationContext context)
    {
        if (context.Time.StepNo >= _maxIterations)
        {
            return SimulationCrashResult.Crash("Maximum iterations reached");
        }

        var environment = context.GetSimulationEnvironment<DifferentialEquationEnvironment>();
        var agents = context.GetAgents<DESolverAgent>();

        if (agents.All(a => a.HasReachedEnd(environment.EndX)))
        {
            return SimulationCrashResult.Crash("All solvers completed");
        }

        return SimulationCrashResult.NoCrash;
    }
}

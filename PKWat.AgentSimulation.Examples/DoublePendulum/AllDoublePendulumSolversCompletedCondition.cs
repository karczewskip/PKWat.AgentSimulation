namespace PKWat.AgentSimulation.Examples.DoublePendulum;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Crash;
using PKWat.AgentSimulation.Examples.DoublePendulum.Agents;

public class AllDoublePendulumSolversCompletedCondition
{
    private readonly long _maxIterations;

    public AllDoublePendulumSolversCompletedCondition(long maxIterations = 100000)
    {
        _maxIterations = maxIterations;
    }

    public SimulationCrashResult CheckCondition(ISimulationContext context)
    {
        if (context.Time.StepNo >= _maxIterations)
        {
            return SimulationCrashResult.Crash("Maximum iterations reached");
        }

        var environment = context.GetSimulationEnvironment<DoublePendulumEnvironment>();
        var agents = context.GetAgents<DoublePendulumSolverAgent>();

        if (agents.All(agent => agent.HasReachedEnd(environment.TotalTime)))
        {
            return SimulationCrashResult.Crash("All double pendulum solvers completed");
        }

        return SimulationCrashResult.NoCrash;
    }
}

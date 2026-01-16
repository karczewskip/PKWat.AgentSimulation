namespace PKWat.AgentSimulation.Examples.Pendulum;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Crash;
using PKWat.AgentSimulation.Examples.Pendulum.Agents;

public class AllPendulumSolversCompletedCondition
{
    private readonly long _maxIterations;

    public AllPendulumSolversCompletedCondition(long maxIterations = 100000)
    {
        _maxIterations = maxIterations;
    }

    public SimulationCrashResult CheckCondition(ISimulationContext context)
    {
        if (context.Time.StepNo >= _maxIterations)
        {
            return SimulationCrashResult.Crash("Maximum iterations reached");
        }

        var environment = context.GetSimulationEnvironment<PendulumEnvironment>();
        var agents = context.GetAgents<PendulumSolverAgent>();

        if (agents.All(agent => agent.HasReachedEnd(environment.TotalTime)))
        {
            return SimulationCrashResult.Crash("All pendulum solvers completed");
        }

        return SimulationCrashResult.NoCrash;
    }
}

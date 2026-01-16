namespace PKWat.AgentSimulation.Examples.Pendulum.Agents;

using PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

public class ExactAnalyticalPendulumAgent : PendulumSolverAgent
{
    public ExactAnalyticalPendulumAgent() : base(new ExactAnalyticalPendulumSolver(), "Exact Analytical")
    {
    }
}

namespace PKWat.AgentSimulation.Examples.Pendulum.Agents;

using PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

public class AnalyticalPendulumAgent : PendulumSolverAgent
{
    public AnalyticalPendulumAgent() : base(new AnalyticalPendulumSolver(), "Analytical")
    {
    }
}

namespace PKWat.AgentSimulation.Examples.Pendulum.Agents;

using PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

public class RK4PendulumAgent : PendulumSolverAgent
{
    public RK4PendulumAgent() : base(new RK4PendulumSolver(), "RK4")
    {
    }
}

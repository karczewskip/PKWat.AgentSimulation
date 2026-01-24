namespace PKWat.AgentSimulation.Examples.DoublePendulum.Agents;

using PKWat.AgentSimulation.SimMath.Algorithms.DoublePendulum;

public class RK4DoublePendulumAgent : DoublePendulumSolverAgent
{
    public RK4DoublePendulumAgent() : base(new RK4DoublePendulumSolver(), "RK4")
    {
    }
}

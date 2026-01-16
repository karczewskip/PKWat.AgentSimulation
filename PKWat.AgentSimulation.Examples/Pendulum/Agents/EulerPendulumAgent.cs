namespace PKWat.AgentSimulation.Examples.Pendulum.Agents;

using PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

public class EulerPendulumAgent : PendulumSolverAgent
{
    public EulerPendulumAgent() : base(new EulerPendulumSolver(), "Euler")
    {
    }
}

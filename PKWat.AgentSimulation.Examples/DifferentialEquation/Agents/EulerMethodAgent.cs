namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

using PKWat.AgentSimulation.SimMath.Algorithms.DifferentialEquations;

public class EulerMethodAgent : DESolverAgent
{
    public EulerMethodAgent() : base(new EulerMethod())
    {
    }
}

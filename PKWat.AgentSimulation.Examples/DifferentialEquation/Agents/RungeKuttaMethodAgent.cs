namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

using PKWat.AgentSimulation.SimMath.Algorithms.DifferentialEquations;

public class RungeKuttaMethodAgent : DESolverAgent
{
    public RungeKuttaMethodAgent() : base(new RungeKuttaMethod())
    {
    }
}

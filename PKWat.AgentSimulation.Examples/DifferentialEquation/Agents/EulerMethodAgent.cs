namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

public class EulerMethodAgent : DESolverAgent
{
    protected override double CalculateNextY(double stepSize)
    {
        double slope = DerivativeFunction!(CurrentX, CurrentY);
        return CurrentY + stepSize * slope;
    }
}

namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

public class RungeKuttaMethodAgent : DESolverAgent
{
    protected override double CalculateNextY(double stepSize)
    {
        double k1 = DerivativeFunction!(CurrentX, CurrentY);
        double k2 = DerivativeFunction!(CurrentX + stepSize / 2, CurrentY + k1 / 2);
        double k3 = DerivativeFunction!(CurrentX + stepSize / 2, CurrentY + k2 / 2);
        double k4 = DerivativeFunction!(CurrentX + stepSize, CurrentY + k3);
        
        return CurrentY + (k1 + 2 * k2 + 2 * k3 + k4) / 6;
    }
}

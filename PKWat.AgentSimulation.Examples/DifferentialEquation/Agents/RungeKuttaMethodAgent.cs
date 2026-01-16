namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

public class RungeKuttaMethodAgent : DESolverAgent
{
    public override void CalculateNextStep(double stepSize)
    {
        double k1 = DerivativeFunction!(CurrentX, CurrentY);
        double k2 = DerivativeFunction!(CurrentX + stepSize / 2, CurrentY + stepSize * k1 / 2);
        double k3 = DerivativeFunction!(CurrentX + stepSize / 2, CurrentY + stepSize * k2 / 2);
        double k4 = DerivativeFunction!(CurrentX + stepSize, CurrentY + stepSize * k3);
        
        CurrentY += stepSize * (k1 + 2 * k2 + 2 * k3 + k4) / 6;
        CurrentX += stepSize;
        SolutionPoints.Add((CurrentX, CurrentY));
    }
}

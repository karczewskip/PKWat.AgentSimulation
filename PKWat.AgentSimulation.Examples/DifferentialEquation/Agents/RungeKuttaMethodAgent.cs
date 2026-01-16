namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

public class RungeKuttaMethodAgent : DESolverAgent
{
    public override void CalculateNextStep(double stepSize, Func<double, double, double> derivativeFunc)
    {
        double k1 = derivativeFunc(CurrentX, CurrentY);
        double k2 = derivativeFunc(CurrentX + stepSize / 2, CurrentY + stepSize * k1 / 2);
        double k3 = derivativeFunc(CurrentX + stepSize / 2, CurrentY + stepSize * k2 / 2);
        double k4 = derivativeFunc(CurrentX + stepSize, CurrentY + stepSize * k3);
        
        CurrentY += stepSize * (k1 + 2 * k2 + 2 * k3 + k4) / 6;
        CurrentX += stepSize;
        SolutionPoints.Add((CurrentX, CurrentY));
    }
}

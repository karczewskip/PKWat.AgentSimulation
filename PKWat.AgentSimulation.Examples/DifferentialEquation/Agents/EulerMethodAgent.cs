namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

public class EulerMethodAgent : DESolverAgent
{
    public override void CalculateNextStep(double stepSize, Func<double, double, double> derivativeFunc)
    {
        double slope = derivativeFunc(CurrentX, CurrentY);
        CurrentY += stepSize * slope;
        CurrentX += stepSize;
        SolutionPoints.Add((CurrentX, CurrentY));
    }
}

namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

public class EulerMethodAgent : DESolverAgent
{
    public override void CalculateNextStep(double stepSize)
    {
        double slope = DerivativeFunction!(CurrentX, CurrentY);
        CurrentY += stepSize * slope;
        CurrentX += stepSize;
        SolutionPoints.Add((CurrentX, CurrentY));
    }
}

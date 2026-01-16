namespace PKWat.AgentSimulation.SimMath.Algorithms.DifferentialEquations;

public class EulerMethod : IDifferentialEquationSolver
{
    public double CalculateNextY(double currentX, double currentY, double stepSize, Func<double, double, double> derivativeFunction)
    {
        double slope = derivativeFunction(currentX, currentY);
        return currentY + stepSize * slope;
    }
}

namespace PKWat.AgentSimulation.SimMath.Algorithms.DifferentialEquations;

public class RungeKuttaMethod : IDifferentialEquationSolver
{
    public double CalculateNextY(double currentX, double currentY, double stepSize, Func<double, double, double> derivativeFunction)
    {
        double k1 = derivativeFunction(currentX, currentY);
        double k2 = derivativeFunction(currentX + stepSize / 2, currentY + stepSize * k1 / 2);
        double k3 = derivativeFunction(currentX + stepSize / 2, currentY + stepSize * k2 / 2);
        double k4 = derivativeFunction(currentX + stepSize, currentY + stepSize * k3);
        
        return currentY + stepSize * (k1 + 2 * k2 + 2 * k3 + k4) / 6;
    }
}

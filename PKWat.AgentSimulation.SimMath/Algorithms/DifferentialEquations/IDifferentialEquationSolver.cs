namespace PKWat.AgentSimulation.SimMath.Algorithms.DifferentialEquations;

public interface IDifferentialEquationSolver
{
    double CalculateNextY(double currentX, double currentY, double stepSize, Func<double, double, double> derivativeFunction);
}

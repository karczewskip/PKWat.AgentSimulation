namespace PKWat.AgentSimulation.SimMath.Algorithms.DifferentialEquations;

public interface IDifferentialEquationSolver
{
    double[] CalculateNextState(double currentT, double[] currentState, double stepSize, Func<double, double[], double[]> derivativeFunction);
}

namespace PKWat.AgentSimulation.SimMath.Algorithms.DifferentialEquations;

public class EulerMethod : IDifferentialEquationSolver
{
    public double[] CalculateNextState(double currentT, double[] currentState, double stepSize, Func<double, double[], double[]> derivativeFunction)
    {
        double[] slopes = derivativeFunction(currentT, currentState);

        int dimension = currentState.Length;
        double[] nextState = new double[dimension];

        for (int i = 0; i < dimension; i++)
        {
            nextState[i] = currentState[i] + stepSize * slopes[i];
        }

        return nextState;
    }
}

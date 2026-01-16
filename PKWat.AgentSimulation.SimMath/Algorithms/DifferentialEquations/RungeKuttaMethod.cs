namespace PKWat.AgentSimulation.SimMath.Algorithms.DifferentialEquations;

public class RungeKuttaMethod : IDifferentialEquationSolver
{
    public double[] CalculateNextState(double currentT, double[] currentState, double stepSize, Func<double, double[], double[]> derivativeFunction)
    {
        int dimension = currentState.Length;
        double[] nextState = new double[dimension];

        double[] k1 = derivativeFunction(currentT, currentState);

        double[] tempStateK2 = new double[dimension];
        for (int i = 0; i < dimension; i++) tempStateK2[i] = currentState[i] + stepSize * k1[i] / 2;
        double[] k2 = derivativeFunction(currentT + stepSize / 2, tempStateK2);

        double[] tempStateK3 = new double[dimension];
        for (int i = 0; i < dimension; i++) tempStateK3[i] = currentState[i] + stepSize * k2[i] / 2;
        double[] k3 = derivativeFunction(currentT + stepSize / 2, tempStateK3);

        double[] tempStateK4 = new double[dimension];
        for (int i = 0; i < dimension; i++) tempStateK4[i] = currentState[i] + stepSize * k3[i];
        double[] k4 = derivativeFunction(currentT + stepSize, tempStateK4);

        for (int i = 0; i < dimension; i++)
        {
            nextState[i] = currentState[i] + stepSize * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]) / 6;
        }

        return nextState;
    }
}

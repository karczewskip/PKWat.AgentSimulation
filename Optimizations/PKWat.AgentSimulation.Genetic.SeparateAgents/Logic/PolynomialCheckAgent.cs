using PKWat.AgentSimulation.Core.Agent;

namespace PKWat.AgentSimulation.Genetic.SeparateAgents.Logic;

internal record PolynomialParameters(int Degree, double[] Coefficients)
{
    public static PolynomialParameters BuildFromCoefficients(double[] coefficients)
    {
        return new PolynomialParameters(coefficients.Length - 1, coefficients);
    }
}

internal record ExpectedValues(double[] X, double[] Y)
{
    public static ExpectedValues Build(double[] X, Func<double, double> function)
    {
        double[] Y = new double[X.Length];
        for (int i = 0; i < X.Length; i++)
        {
            Y[i] = function(X[i]);
        }
        return new ExpectedValues(X, Y);
    }
}

internal record ErrorResult(double AbsoluteError);

internal class PolynomialCheckAgent() : SimpleSimulationAgent
{
    public PolynomialParameters? Parameters { get; private set; }

    public void SetParameters(PolynomialParameters parameters)
    {
        Parameters = parameters;
    }

    public ErrorResult CalculateError(ExpectedValues expectedValues)
    {
        if(Parameters == null)
        {
            throw new InvalidOperationException("Parameters must be set before calculating error.");
        }

        double absoluteError = 0;

        for (int i = 0; i < expectedValues.X.Length; i++)
        {
            double x = expectedValues.X[i];
            double predictedY = 0;
            for (int d = 0; d <= Parameters.Degree; d++)
            {
                predictedY *= x;
                predictedY += Parameters.Coefficients[d];
            }
            double absError = Math.Abs(predictedY - expectedValues.Y[i]);
            absoluteError += absError;
        }

        return new ErrorResult(absoluteError);
    }
}

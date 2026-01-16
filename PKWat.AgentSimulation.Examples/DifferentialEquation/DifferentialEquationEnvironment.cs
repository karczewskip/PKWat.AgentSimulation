namespace PKWat.AgentSimulation.Examples.DifferentialEquation;

using PKWat.AgentSimulation.Core.Environment;

public class DifferentialEquationEnvironment : DefaultSimulationEnvironment
{
    public double StepSize { get; private set; } = 0.1;
    public double StartX { get; private set; } = 0.0;
    public double EndX { get; private set; } = 5.0;
    public double InitialY { get; private set; } = 9.0;

    public void SetParameters(double stepSize)
    {
        StepSize = stepSize;
    }

    public double DerivativeFunction(double x, double y)
    {
        //return y - x * x;
        return 2 * (x - 2);
    }

    public double AnalyticalSolution(double x)
    {
        //return 2 + 2 * x + x * x - System.Math.Exp(x);
        return x * x - 4 * x + 9;
    }
}

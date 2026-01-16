namespace PKWat.AgentSimulation.Examples.DifferentialEquation;

using PKWat.AgentSimulation.Core.Environment;

public class DifferentialEquationEnvironment : DefaultSimulationEnvironment
{
    public double StepSize { get; private set; } = 0.1;
    public double StartX { get; private set; } = 0.0;
    public double EndX { get; private set; } = 10.0;
    public double InitialY { get; private set; } = 1.0;

    public void SetParameters(double stepSize, double startX, double endX, double initialY)
    {
        StepSize = stepSize;
        StartX = startX;
        EndX = endX;
        InitialY = initialY;
    }

    public double DerivativeFunction(double x, double y)
    {
        return y;
    }

    public double AnalyticalSolution(double x)
    {
        return InitialY * System.Math.Exp(x - StartX);
    }
}

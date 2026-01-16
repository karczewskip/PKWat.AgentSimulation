namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

using PKWat.AgentSimulation.Core.Agent;

public abstract class DESolverAgent : SimpleSimulationAgent
{
    public double CurrentX { get; protected set; }
    public double CurrentY { get; protected set; }
    public List<(double X, double Y)> SolutionPoints { get; } = new();
    protected Func<double, double, double>? DerivativeFunction { get; private set; }

    public void Initialize(double startX, double initialY)
    {
        CurrentX = startX;
        CurrentY = initialY;
        SolutionPoints.Clear();
        SolutionPoints.Add((startX, initialY));
    }

    public void SetDerivativeFunction(Func<double, double, double> derivativeFunc)
    {
        DerivativeFunction = derivativeFunc;
    }

    public abstract void CalculateNextStep(double stepSize);

    public bool HasReachedEnd(double endX)
    {
        return CurrentX >= endX;
    }
}

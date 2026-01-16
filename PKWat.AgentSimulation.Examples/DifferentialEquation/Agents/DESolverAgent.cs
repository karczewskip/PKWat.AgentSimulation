namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.SimMath.Algorithms.DifferentialEquations;

public class DESolverAgent : SimpleSimulationAgent
{
    private readonly IDifferentialEquationSolver _solver;
    
    public double CurrentX { get; protected set; }
    public double CurrentY { get; protected set; }
    public List<(double X, double Y)> SolutionPoints { get; } = new();
    protected Func<double, double, double>? DerivativeFunction { get; private set; }

    public DESolverAgent(IDifferentialEquationSolver solver)
    {
        _solver = solver;
    }

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

    public void CalculateNextStep(double stepSize)
    {
        CurrentY = _solver.CalculateNextY(CurrentX, CurrentY, stepSize, DerivativeFunction!);
        CurrentX += stepSize;
        SolutionPoints.Add((CurrentX, CurrentY));
    }

    public bool HasReachedEnd(double endX)
    {
        return CurrentX >= endX;
    }
}

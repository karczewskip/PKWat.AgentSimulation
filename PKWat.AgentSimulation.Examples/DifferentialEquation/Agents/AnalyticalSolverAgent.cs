namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

using PKWat.AgentSimulation.Core.Agent;

public class AnalyticalSolverAgent : SimpleSimulationAgent
{
    private Func<double, double>? _analyticalSolution;
    
    public double CurrentX { get; private set; }
    public double CurrentY { get; private set; }
    public List<(double X, double Y)> SolutionPoints { get; } = new();

    public void Initialize(double startX, double initialY)
    {
        CurrentX = startX;
        CurrentY = initialY;
        SolutionPoints.Clear();
        SolutionPoints.Add((startX, initialY));
    }

    public void SetAnalyticalSolution(Func<double, double> solution)
    {
        _analyticalSolution = solution;
    }

    public void CalculateNextStep(double stepSize)
    {
        CurrentX += stepSize;
        CurrentY = _analyticalSolution!(CurrentX);
        SolutionPoints.Add((CurrentX, CurrentY));
    }

    public bool HasReachedEnd(double endX)
    {
        return CurrentX >= endX;
    }
}

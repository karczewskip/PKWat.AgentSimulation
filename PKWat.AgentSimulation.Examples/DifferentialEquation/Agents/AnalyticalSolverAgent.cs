namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

public class AnalyticalSolverAgent : DESolverAgent
{
    private Func<double, double>? _analyticalSolution;

    public void SetAnalyticalSolution(Func<double, double> solution)
    {
        _analyticalSolution = solution;
    }

    public override void CalculateNextStep(double stepSize)
    {
        CurrentX += stepSize;
        CurrentY = _analyticalSolution!(CurrentX);
        SolutionPoints.Add((CurrentX, CurrentY));
    }
}

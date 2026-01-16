namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

public class AnalyticalSolverAgent : DESolverAgent
{
    private Func<double, double>? _analyticalSolution;

    public void SetAnalyticalSolution(Func<double, double> solution)
    {
        _analyticalSolution = solution;
    }

    protected override double CalculateNextY(double stepSize)
    {
        return _analyticalSolution!(CurrentX);
    }
}

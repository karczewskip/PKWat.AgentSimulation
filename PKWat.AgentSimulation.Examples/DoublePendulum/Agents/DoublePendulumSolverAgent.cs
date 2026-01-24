namespace PKWat.AgentSimulation.Examples.DoublePendulum.Agents;

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.SimMath.Algorithms.DoublePendulum;

public class DoublePendulumSolverAgent : SimpleSimulationAgent
{
    private readonly IDoublePendulumSolver _solver;
    
    public double CurrentTime { get; private set; }
    public DoublePendulumState CurrentState { get; private set; } = new DoublePendulumState(0, 0, 0, 0);
    public List<(double Time, DoublePendulumState State, double X1, double Y1, double X2, double Y2)> StateHistory { get; } = new();
    public string SolverName { get; }

    public DoublePendulumSolverAgent(IDoublePendulumSolver solver, string solverName)
    {
        _solver = solver;
        SolverName = solverName;
    }

    public void Initialize(double initialTheta1, double initialOmega1, double initialTheta2, double initialOmega2, double L1, double L2)
    {
        CurrentTime = 0.0;
        CurrentState = new DoublePendulumState(initialTheta1, initialOmega1, initialTheta2, initialOmega2);
        StateHistory.Clear();
        
        // Calculate initial positions
        double x1 = L1 * Math.Sin(initialTheta1);
        double y1 = -L1 * Math.Cos(initialTheta1);
        double x2 = x1 + L2 * Math.Sin(initialTheta2);
        double y2 = y1 - L2 * Math.Cos(initialTheta2);
        
        StateHistory.Add((CurrentTime, CurrentState, x1, y1, x2, y2));
    }

    public void CalculateNextStep(double dt, double g, double L1, double L2, double m1, double m2)
    {
        CurrentTime += dt;
        CurrentState = _solver.CalculateNextState(CurrentState, dt, g, L1, L2, m1, m2);
        
        // Calculate positions
        double x1 = L1 * Math.Sin(CurrentState.Theta1);
        double y1 = -L1 * Math.Cos(CurrentState.Theta1);
        double x2 = x1 + L2 * Math.Sin(CurrentState.Theta2);
        double y2 = y1 - L2 * Math.Cos(CurrentState.Theta2);
        
        StateHistory.Add((CurrentTime, CurrentState, x1, y1, x2, y2));
    }

    public bool HasReachedEnd(double totalTime)
    {
        return CurrentTime >= totalTime;
    }
}

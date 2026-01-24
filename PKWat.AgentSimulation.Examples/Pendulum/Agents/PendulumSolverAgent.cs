namespace PKWat.AgentSimulation.Examples.Pendulum.Agents;

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

public class PendulumSolverAgent : SimpleSimulationAgent
{
    private readonly IPendulumSolver _solver;
    
    public double CurrentTime { get; private set; }
    public PendulumState CurrentState { get; private set; } = new PendulumState(0, 0);
    public List<(double Time, double Theta, double Omega)> StateHistory { get; } = new();
    public string SolverName { get; }

    public PendulumSolverAgent(IPendulumSolver solver, string solverName)
    {
        _solver = solver;
        SolverName = solverName;
    }

    public void Initialize(double initialTheta, double initialOmega)
    {
        CurrentTime = 0.0;
        CurrentState = new PendulumState(initialTheta, initialOmega);
        StateHistory.Clear();
        StateHistory.Add((CurrentTime, CurrentState.Theta, CurrentState.Omega));

        if (_solver is AnalyticalPendulumSolver analyticalSolver)
        {
            analyticalSolver.Initialize(initialTheta);
        }
        else if (_solver is ExactAnalyticalPendulumSolver exactAnalyticalSolver)
        {
            exactAnalyticalSolver.Initialize(initialTheta);
        }
    }

    public void CalculateNextStep(double dt, double g, double L)
    {
        CurrentTime += dt;
        CurrentState = _solver.CalculateNextState(CurrentState, CurrentTime, dt, g, L);
        StateHistory.Add((CurrentTime, CurrentState.Theta, CurrentState.Omega));
    }

    public bool HasReachedEnd(double totalTime)
    {
        return CurrentTime >= totalTime;
    }
}

namespace PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

public interface IPendulumSolver
{
    PendulumState CalculateNextState(PendulumState currentState, double time, double dt, double g, double L);
}

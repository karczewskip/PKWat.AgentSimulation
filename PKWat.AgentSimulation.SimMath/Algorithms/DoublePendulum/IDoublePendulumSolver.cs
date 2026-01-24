namespace PKWat.AgentSimulation.SimMath.Algorithms.DoublePendulum;

public interface IDoublePendulumSolver
{
    DoublePendulumState CalculateNextState(DoublePendulumState currentState, double dt, double g, double L1, double L2, double m1, double m2);
}

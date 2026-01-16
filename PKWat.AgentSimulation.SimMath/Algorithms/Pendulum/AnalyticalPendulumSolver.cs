namespace PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

public class AnalyticalPendulumSolver : IPendulumSolver
{
    private double _initialTheta;

    public void Initialize(double initialTheta)
    {
        _initialTheta = initialTheta;
    }

    public PendulumState CalculateNextState(PendulumState currentState, double time, double dt, double g, double L)
    {
        double omega0 = Math.Sqrt(g / L);
        double theta = _initialTheta * Math.Cos(omega0 * time);
        double omega = -_initialTheta * omega0 * Math.Sin(omega0 * time);
        
        return new PendulumState(theta, omega);
    }
}

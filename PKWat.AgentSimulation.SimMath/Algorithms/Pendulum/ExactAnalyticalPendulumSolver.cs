namespace PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

/// <summary>
/// Exact analytical solution for a simple pendulum using Jacobi Elliptic functions.
/// This solution is valid for any initial angle without small-angle approximation.
/// </summary>
public class ExactAnalyticalPendulumSolver : IPendulumSolver
{
    private double _initialTheta;
    private double _k;
    private double _K;

    public void Initialize(double initialTheta)
    {
        _initialTheta = initialTheta;
        _k = Math.Sin(Math.Abs(initialTheta) / 2.0);
        _K = JacobiElliptic.CompleteEllipticIntegralFirstKind(_k);
    }

    public PendulumState CalculateNextState(PendulumState currentState, double time, double dt, double g, double L)
    {
        double omega0 = Math.Sqrt(g / L);
        
        // Exact analytical solution using Jacobi Elliptic functions
        // For a pendulum starting from rest at angle theta_0:
        // theta(t) = 2 * arcsin(k * sn(omega_0 * t, k))
        // where k = sin(theta_0 / 2)
        // 
        // The phase is adjusted so that at t=0, sn(0, k) = 0 => theta(0) = 0
        // But we want theta(0) = theta_0, so we use:
        // theta(t) = 2 * arcsin(k * sn(omega_0 * t + K(k), k))
        // At t=0: sn(K(k), k) = 1 => theta(0) = 2*arcsin(k) = theta_0
        
        double u = omega0 * time + _K;
        var (sn, cn, dn) = JacobiElliptic.CalculateAll(u, _k);
        
        double theta = 2.0 * Math.Asin(_k * sn);

        // Analytical derivative: d(theta)/dt = 2*k*omega_0*cn(u,k)*dn(u,k) / sqrt(1 - k^2*sn^2(u,k))
        double omega = 2.0 * _k * omega0 * cn;
        
        // Handle the sign based on initial condition
        if (_initialTheta < 0)
        {
            theta = -theta;
            omega = -omega;
        }
        
        return new PendulumState(theta, omega);
    }
}

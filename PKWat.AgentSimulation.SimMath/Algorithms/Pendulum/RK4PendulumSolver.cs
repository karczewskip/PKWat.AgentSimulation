namespace PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

public class RK4PendulumSolver : IPendulumSolver
{
    public PendulumState CalculateNextState(PendulumState currentState, double time, double dt, double g, double L)
    {
        var k1 = CalculateDerivatives(currentState, g, L);
        
        var state2 = new PendulumState(
            currentState.Theta + k1.dTheta * dt / 2,
            currentState.Omega + k1.dOmega * dt / 2
        );
        var k2 = CalculateDerivatives(state2, g, L);
        
        var state3 = new PendulumState(
            currentState.Theta + k2.dTheta * dt / 2,
            currentState.Omega + k2.dOmega * dt / 2
        );
        var k3 = CalculateDerivatives(state3, g, L);
        
        var state4 = new PendulumState(
            currentState.Theta + k3.dTheta * dt,
            currentState.Omega + k3.dOmega * dt
        );
        var k4 = CalculateDerivatives(state4, g, L);
        
        double newTheta = currentState.Theta + (dt / 6.0) * (k1.dTheta + 2 * k2.dTheta + 2 * k3.dTheta + k4.dTheta);
        double newOmega = currentState.Omega + (dt / 6.0) * (k1.dOmega + 2 * k2.dOmega + 2 * k3.dOmega + k4.dOmega);
        
        return new PendulumState(newTheta, newOmega);
    }

    private (double dTheta, double dOmega) CalculateDerivatives(PendulumState state, double g, double L)
    {
        double dTheta = state.Omega;
        double dOmega = -(g / L) * Math.Sin(state.Theta);
        return (dTheta, dOmega);
    }
}

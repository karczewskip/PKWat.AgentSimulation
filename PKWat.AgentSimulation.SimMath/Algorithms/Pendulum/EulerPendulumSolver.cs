namespace PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

public class EulerPendulumSolver : IPendulumSolver
{
    public PendulumState CalculateNextState(PendulumState currentState, double time, double dt, double g, double L)
    {
        double dTheta = currentState.Omega;
        double dOmega = -(g / L) * Math.Sin(currentState.Theta);
        
        double newTheta = currentState.Theta + dTheta * dt;
        double newOmega = currentState.Omega + dOmega * dt;
        
        return new PendulumState(newTheta, newOmega);
    }
}

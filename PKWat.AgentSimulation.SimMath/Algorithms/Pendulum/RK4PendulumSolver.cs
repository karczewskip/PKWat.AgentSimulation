namespace PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

using PKWat.AgentSimulation.SimMath.Algorithms.DifferentialEquations;

public class RK4PendulumSolver : IPendulumSolver
{
    private readonly RungeKuttaMethod _rk4Method = new();

    public PendulumState CalculateNextState(PendulumState currentState, double time, double dt, double g, double L)
    {
        // Convert pendulum state to array: [theta, omega]
        double[] state = [currentState.Theta, currentState.Omega];

        // Define derivative function for the pendulum system
        // d(theta)/dt = omega
        // d(omega)/dt = -(g/L) * sin(theta)
        double[] newState = _rk4Method.CalculateNextState(
            time,
            state,
            dt,
            (t, s) => [s[1], -(g / L) * Math.Sin(s[0])]
        );

        return new PendulumState(newState[0], newState[1]);
    }
}

namespace PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

using PKWat.AgentSimulation.SimMath.Algorithms.DifferentialEquations;

public class EulerPendulumSolver : IPendulumSolver
{
    private readonly EulerMethod _eulerMethod = new();

    public PendulumState CalculateNextState(PendulumState currentState, double time, double dt, double g, double L)
    {
        double[] state = [currentState.Theta, currentState.Omega];

        double[] newState = _eulerMethod.CalculateNextState(
            time,
            state,
            dt,
            (t, s) => [s[1], -(g / L) * Math.Sin(s[0])]
        );

        return new PendulumState(newState[0], newState[1]);
    }
}

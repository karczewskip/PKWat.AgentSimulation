namespace PKWat.AgentSimulation.SimMath.Algorithms.Pendulum;

public class PendulumState
{
    public double Theta { get; set; }
    public double Omega { get; set; }

    public PendulumState(double theta, double omega)
    {
        Theta = theta;
        Omega = omega;
    }

    public PendulumState Clone()
    {
        return new PendulumState(Theta, Omega);
    }
}

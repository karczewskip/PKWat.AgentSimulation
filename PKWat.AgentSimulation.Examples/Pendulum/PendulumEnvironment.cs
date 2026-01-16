namespace PKWat.AgentSimulation.Examples.Pendulum;

using PKWat.AgentSimulation.Core.Environment;

public class PendulumEnvironment : DefaultSimulationEnvironment
{
    public double Gravity { get; private set; } = 9.81;
    public double Length { get; private set; } = 1.0;
    public double InitialTheta { get; private set; } = Math.PI / 4;
    public double InitialOmega { get; private set; } = 0.0;
    public double TimeStep { get; private set; } = 0.01;
    public double TotalTime { get; private set; } = 10.0;

    public void SetParameters(double gravity, double length, double initialTheta, double initialOmega, double timeStep, double totalTime)
    {
        Gravity = gravity;
        Length = length;
        InitialTheta = initialTheta;
        InitialOmega = initialOmega;
        TimeStep = timeStep;
        TotalTime = totalTime;
    }
}

namespace PKWat.AgentSimulation.Examples.DoublePendulum;

using PKWat.AgentSimulation.Core.Environment;

public class DoublePendulumEnvironment : DefaultSimulationEnvironment
{
    public double Gravity { get; private set; } = 9.81;
    public double Length1 { get; private set; } = 1.0;
    public double Length2 { get; private set; } = 1.0;
    public double Mass1 { get; private set; } = 1.0;
    public double Mass2 { get; private set; } = 1.0;
    public double InitialTheta1 { get; private set; } = Math.PI / 2;
    public double InitialOmega1 { get; private set; } = 0.0;
    public double InitialTheta2 { get; private set; } = Math.PI / 2;
    public double InitialOmega2 { get; private set; } = 0.0;
    public double TimeStep { get; private set; } = 0.01;
    public double TotalTime { get; private set; } = 10.0;

    public void SetParameters(double gravity, double length1, double length2, double mass1, double mass2,
        double initialTheta1, double initialOmega1, double initialTheta2, double initialOmega2,
        double timeStep, double totalTime)
    {
        Gravity = gravity;
        Length1 = length1;
        Length2 = length2;
        Mass1 = mass1;
        Mass2 = mass2;
        InitialTheta1 = initialTheta1;
        InitialOmega1 = initialOmega1;
        InitialTheta2 = initialTheta2;
        InitialOmega2 = initialOmega2;
        TimeStep = timeStep;
        TotalTime = totalTime;
    }
}

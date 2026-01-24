namespace PKWat.AgentSimulation.Examples.DoublePendulum.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class InitializeDoublePendulumParameters : ISimulationStage
{
    private double _gravity = 9.81;
    private double _length1 = 1.0;
    private double _length2 = 1.0;
    private double _mass1 = 1.0;
    private double _mass2 = 1.0;
    private double _initialTheta1 = Math.PI / 2;
    private double _initialOmega1 = 0.0;
    private double _initialTheta2 = Math.PI / 2;
    private double _initialOmega2 = 0.0;
    private double _timeStep = 0.01;
    private double _totalTime = 10.0;

    public void SetParameters(double gravity, double length1, double length2, double mass1, double mass2,
        double initialTheta1, double initialOmega1, double initialTheta2, double initialOmega2,
        double timeStep, double totalTime)
    {
        _gravity = gravity;
        _length1 = length1;
        _length2 = length2;
        _mass1 = mass1;
        _mass2 = mass2;
        _initialTheta1 = initialTheta1;
        _initialOmega1 = initialOmega1;
        _initialTheta2 = initialTheta2;
        _initialOmega2 = initialOmega2;
        _timeStep = timeStep;
        _totalTime = totalTime;
    }

    public Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<DoublePendulumEnvironment>();
        environment.SetParameters(_gravity, _length1, _length2, _mass1, _mass2,
            _initialTheta1, _initialOmega1, _initialTheta2, _initialOmega2,
            _timeStep, _totalTime);
        return Task.CompletedTask;
    }
}

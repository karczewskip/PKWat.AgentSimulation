namespace PKWat.AgentSimulation.Examples.Pendulum.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class InitializePendulumParameters : ISimulationStage
{
    private double _gravity = 9.81;
    private double _length = 1.0;
    private double _initialTheta = Math.PI / 4;
    private double _initialOmega = 0.0;
    private double _timeStep = 0.01;
    private double _totalTime = 10.0;

    public void SetParameters(double gravity, double length, double initialTheta, double initialOmega, double timeStep, double totalTime)
    {
        _gravity = gravity;
        _length = length;
        _initialTheta = initialTheta;
        _initialOmega = initialOmega;
        _timeStep = timeStep;
        _totalTime = totalTime;
    }

    public Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<PendulumEnvironment>();
        environment.SetParameters(_gravity, _length, _initialTheta, _initialOmega, _timeStep, _totalTime);
        return Task.CompletedTask;
    }
}

namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class InitializeParameters : ISimulationStage
{
    private double _stepSize = 0.1;
    private double _startX = 0.0;
    private double _endX = 10.0;
    private double _initialY = 1.0;

    public void SetParameters(double stepSize, double startX, double endX, double initialY)
    {
        _stepSize = stepSize;
        _startX = startX;
        _endX = endX;
        _initialY = initialY;
    }

    public Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<DifferentialEquationEnvironment>();
        environment.SetParameters(_stepSize, _startX, _endX, _initialY);
        return Task.CompletedTask;
    }
}

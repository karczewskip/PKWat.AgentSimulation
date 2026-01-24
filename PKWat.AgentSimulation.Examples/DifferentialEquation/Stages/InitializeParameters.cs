namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

public class InitializeParameters : ISimulationStage
{
    private double _stepSize = 0.1;

    public void SetParameters(double stepSize)
    {
        _stepSize = stepSize;
    }

    public Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<DifferentialEquationEnvironment>();
        environment.SetParameters(_stepSize);
        return Task.CompletedTask;
    }
}

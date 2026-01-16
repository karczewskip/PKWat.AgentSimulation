namespace PKWat.AgentSimulation.Examples.DifferentialEquation;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;
using PKWat.AgentSimulation.Examples.DifferentialEquation.Stages;

public class DifferentialEquationSimulationBuilder
{
    private readonly ISimulationBuilder _simulationBuilder;

    public DifferentialEquationSimulationBuilder(ISimulationBuilder simulationBuilder)
    {
        _simulationBuilder = simulationBuilder;
    }

    public ISimulation Build(double stepSize = 0.1, double startX = 0.0, double endX = 10.0, double initialY = 1.0)
    {
        var crashCondition = new AllSolversCompletedCondition();

        var simulation = _simulationBuilder
            .CreateNewSimulation<DifferentialEquationEnvironment>()
            .AddInitializationStage<InitializeParameters>(s => s.SetParameters(stepSize, startX, endX, initialY))
            .AddInitializationStage<InitializeSolvers>()
            .AddStage<CalculateNextStep>()
            .AddAgent<AnalyticalSolverAgent>()
            .AddAgent<EulerMethodAgent>()
            .AddAgent<RungeKuttaMethodAgent>()
            .AddCrashCondition(crashCondition.CheckCondition)
            .SetRandomSeed(42)
            .Build();

        return simulation;
    }
}

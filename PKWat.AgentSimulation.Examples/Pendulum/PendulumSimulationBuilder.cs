namespace PKWat.AgentSimulation.Examples.Pendulum;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.Pendulum.Agents;
using PKWat.AgentSimulation.Examples.Pendulum.Stages;

public class PendulumSimulationBuilder
{
    private readonly ISimulationBuilder _simulationBuilder;

    public PendulumSimulationBuilder(ISimulationBuilder simulationBuilder)
    {
        _simulationBuilder = simulationBuilder;
    }

    public ISimulation Build(
        double gravity = 9.81,
        double length = 1.0,
        double initialTheta = Math.PI / 4,
        double initialOmega = 0.0,
        double timeStep = 0.01,
        double totalTime = 10.0)
    {
        var completionCondition = new AllPendulumSolversCompletedCondition();

        var simulation = _simulationBuilder
            .CreateNewSimulation<PendulumEnvironment>()
            .AddInitializationStage<InitializePendulumParameters>(s => 
                s.SetParameters(gravity, length, initialTheta, initialOmega, timeStep, totalTime))
            .AddInitializationStage<InitializePendulumSolvers>()
            .AddStage<CalculatePendulumStep>()
            .AddAgent<AnalyticalPendulumAgent>()
            .AddAgent<ExactAnalyticalPendulumAgent>()
            .AddAgent<EulerPendulumAgent>()
            .AddAgent<RK4PendulumAgent>()
            .AddCrashCondition(completionCondition.CheckCondition)
            .SetRandomSeed(42)
            .Build();

        return simulation;
    }
}

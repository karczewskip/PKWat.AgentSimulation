namespace PKWat.AgentSimulation.Examples.DoublePendulum;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.DoublePendulum.Agents;
using PKWat.AgentSimulation.Examples.DoublePendulum.Stages;

public class DoublePendulumSimulationBuilder
{
    private readonly ISimulationBuilder _simulationBuilder;

    public DoublePendulumSimulationBuilder(ISimulationBuilder simulationBuilder)
    {
        _simulationBuilder = simulationBuilder;
    }

    public ISimulation Build(
        double gravity = 9.81,
        double length1 = 1.0,
        double length2 = 1.0,
        double mass1 = 1.0,
        double mass2 = 1.0,
        double initialTheta1 = Math.PI / 2,
        double initialOmega1 = 0.0,
        double initialTheta2 = Math.PI / 2,
        double initialOmega2 = 0.0,
        double timeStep = 0.01,
        double totalTime = 10.0)
    {
        var completionCondition = new AllDoublePendulumSolversCompletedCondition();

        var simulation = _simulationBuilder
            .CreateNewSimulation<DoublePendulumEnvironment>()
            .AddInitializationStage<InitializeDoublePendulumParameters>(s => 
                s.SetParameters(gravity, length1, length2, mass1, mass2,
                    initialTheta1, initialOmega1, initialTheta2, initialOmega2,
                    timeStep, totalTime))
            .AddInitializationStage<InitializeDoublePendulumSolvers>()
            .AddStage<CalculateDoublePendulumStep>()
            .AddAgent<RK4DoublePendulumAgent>()
            .AddCrashCondition(completionCondition.CheckCondition)
            .SetRandomSeed(42)
            .Build();

        return simulation;
    }
}

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.DoublePendulum;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.DoublePendulum;
using PKWat.AgentSimulation.Examples.DoublePendulum.Agents;
using PKWat.AgentSimulation.Examples.DoublePendulum.Stages;
using System;
using System.Windows.Media.Imaging;

public class DoublePendulumSimulationBuilder(ISimulationBuilder simulationBuilder, DoublePendulumDrawer drawer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        var completionCondition = new AllDoublePendulumSolversCompletedCondition();

        double gravity = 9.81;
        double length1 = 1.0;
        double length2 = 1.0;
        double mass1 = 1.0;
        double mass2 = 1.0;
        double initialTheta1 = Math.PI / 2;
        double initialOmega1 = 0.0;
        double initialTheta2 = Math.PI / 2;
        double initialOmega2 = 0.0;
        double timeStep = 0.01;
        double totalTime = 20.0;

        var simulation = simulationBuilder
            .CreateNewSimulation<DoublePendulumEnvironment>()
            .AddInitializationStage<InitializeDoublePendulumParameters>(s => 
                s.SetParameters(gravity, length1, length2, mass1, mass2,
                    initialTheta1, initialOmega1, initialTheta2, initialOmega2,
                    timeStep, totalTime))
            .AddAgent<RK4DoublePendulumAgent>()
            .AddInitializationStage<InitializeDoublePendulumSolvers>()
            .AddStage<CalculateDoublePendulumStep>()
            .AddCrashCondition(completionCondition.CheckCondition)
            .AddCallback(c => drawing(drawer.Draw(c)))
            .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(10))
            .SetRandomSeed(42)
            .Build();

        return simulation;
    }
}

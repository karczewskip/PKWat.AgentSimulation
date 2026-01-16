namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Pendulum;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.Pendulum;
using PKWat.AgentSimulation.Examples.Pendulum.Agents;
using PKWat.AgentSimulation.Examples.Pendulum.Stages;
using System;
using System.Windows.Media.Imaging;

public class PendulumSimulationBuilder(ISimulationBuilder simulationBuilder, PendulumDrawer drawer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        var completionCondition = new AllPendulumSolversCompletedCondition();

        double gravity = 9.81;
        double length = 1.0;
        double initialTheta = Math.PI / 4;
        double initialOmega = 0.0;
        double timeStep = 0.01;
        double totalTime = 10.0;

        var simulation = simulationBuilder
            .CreateNewSimulation<PendulumEnvironment>()
            .AddInitializationStage<InitializePendulumParameters>(s => 
                s.SetParameters(gravity, length, initialTheta, initialOmega, timeStep, totalTime))
            .AddAgent<AnalyticalPendulumAgent>()
            .AddAgent<EulerPendulumAgent>()
            .AddAgent<RK4PendulumAgent>()
            .AddInitializationStage<InitializePendulumSolvers>()
            .AddStage<CalculatePendulumStep>()
            .AddCrashCondition(completionCondition.CheckCondition)
            .AddCallback(c => drawing(drawer.Draw(c)))
            .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(10))
            .SetRandomSeed(42)
            .Build();

        return simulation;
    }
}

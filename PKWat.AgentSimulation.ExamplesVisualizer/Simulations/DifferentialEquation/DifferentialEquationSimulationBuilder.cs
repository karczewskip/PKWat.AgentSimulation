namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.DifferentialEquation;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.DifferentialEquation;
using PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;
using PKWat.AgentSimulation.Examples.DifferentialEquation.Stages;
using System;
using System.Windows.Media.Imaging;

public class DifferentialEquationSimulationBuilder(ISimulationBuilder simulationBuilder, DifferentialEquationDrawer drawer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        var crashCondition = new AllSolversCompletedCondition();

        var simulation = simulationBuilder
            .CreateNewSimulation<DifferentialEquationEnvironment>()
            .AddInitializationStage<InitializeParameters>(s => s.SetParameters(0.1, 0.0, 5.0, 1.0))
            .AddInitializationStage<InitializeSolvers>()
            .AddStage<CalculateNextStep>()
            .AddAgent<AnalyticalSolverAgent>()
            .AddAgent<EulerMethodAgent>()
            .AddAgent<RungeKuttaMethodAgent>()
            .AddCrashCondition(crashCondition.CheckCondition)
            .AddCallback(c => drawing(drawer.Draw(c)))
            .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(50))
            .SetRandomSeed(42)
            .Build();

        return simulation;
    }
}

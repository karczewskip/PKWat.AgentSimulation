using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Genetics.PolynomialInterpolation;
using PKWat.AgentSimulation.Genetics.PolynomialInterpolation.Stages;
using System.Windows.Media.Imaging;

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.GeneticsPolynomialInterpolation;

public class GeneticsPolynomialInterpolationOnGPUBuilder(ISimulationBuilder simulationBuilder, GeneticsPolynomialAproximationDrawer drawer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing) =>
        simulationBuilder.CreateNewSimulation<CalculationsBlackboard>()
        .AddInitializationStage<InitializeBlackboard>()
        .AddStage<BuildNewAgents>()
        .AddStage<CalculateForAllAgentsByGPU>()
        .AddCallback(c => {
            if (c.GetSimulationEnvironment<CalculationsBlackboard>().ImprovedFromLastCheck)
            {
                drawing(drawer.Draw(c));
            }
        })
        .Build();
}

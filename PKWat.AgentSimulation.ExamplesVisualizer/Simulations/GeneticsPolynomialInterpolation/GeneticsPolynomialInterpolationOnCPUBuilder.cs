using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Genetics.PolynomialAproximation;
using PKWat.AgentSimulation.Genetics.PolynomialAproximation.Stages;
using System.Windows.Media.Imaging;

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.GeneticsPolynomialInterpolation;

public class GeneticsPolynomialInterpolationOnCPUBuilder(ISimulationBuilder simulationBuilder, GeneticsPolynomialAproximationDrawer drawer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing) =>
        simulationBuilder.CreateNewSimulation<CalculationsBlackboard>()
        .AddInitializationStage<InitializeBlackboard>()
        .AddStage<BuildNewAgents>()
        .AddStage<CalculateForAllAgents>()
        .AddCallback(c => {
            if(c.GetSimulationEnvironment<CalculationsBlackboard>().ImprovedFromLastCheck)
            {
                drawing(drawer.Draw(c));
            }
        })
        .Build();
}

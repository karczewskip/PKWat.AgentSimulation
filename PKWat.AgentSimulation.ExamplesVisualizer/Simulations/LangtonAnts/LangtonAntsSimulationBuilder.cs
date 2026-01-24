namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.LangtonAnts;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.LangtonAnts;
using PKWat.AgentSimulation.Examples.LangtonAnts.Stages;
using System.Windows.Media.Imaging;

public class LangtonAntsSimulationBuilder : IExampleSimulationBuilder
{
    private readonly ISimulationBuilder _simulationBuilder;
    private readonly LangtonAntsDrawer _drawer;

    public LangtonAntsSimulationBuilder(ISimulationBuilder simulationBuilder, LangtonAntsDrawer drawer)
    {
        _simulationBuilder = simulationBuilder;
        _drawer = drawer;
    }

    public ISimulation Build(Action<BitmapSource> drawing)
    {
        var simulation = _simulationBuilder
            .CreateNewSimulation<LangtonAntsEnvironment>()
            .AddInitializationStage<InitializeLangtonAnts>(s =>
            {
                s.UseSize(150, 150);
                s.UsePairCount(4);
            })
            .AddStage<CalculateAntMovement>()
            .AddCallback(c => drawing(_drawer.Draw(c)))
            .SetRandomSeed(42)
            .Build();

        return simulation;
    }
}

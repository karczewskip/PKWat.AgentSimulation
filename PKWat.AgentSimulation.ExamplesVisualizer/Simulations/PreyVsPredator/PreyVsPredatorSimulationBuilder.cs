namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator.Agents;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator.Stages;
using System.Windows.Media.Imaging;

public class PreyVsPredatorSimulationBuilder(
    ISimulationBuilder simulationBuilder,
    PreyVsPredatorDrawer drawer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        var simulation = simulationBuilder
            .CreateNewSimulation<PreyVsPredatorEnvironment>()
            .AddAgents<Prey>(1000)
            .AddAgents<Predator>(1000)
            .AddInitializationStage<ActorsPlaced>(c => c.SetSize(200, 200))
            .AddStage<PredatorsStarved>(c => c.ChangeStarvationIncrement(0.001))
            .AddStage<MovedPreyers>()
            .AddStage<MovedPredators>()
            .AddStage<BornNewPreyers>(c => c.ChangePregnancyUpdate(0.1))
            .AddStage<PreyersEaten>()
            .AddCallback(c => drawing(drawer.Draw(c)))
            //.SetRandomSeed(200)
            .Build();

        return simulation;
    }
}

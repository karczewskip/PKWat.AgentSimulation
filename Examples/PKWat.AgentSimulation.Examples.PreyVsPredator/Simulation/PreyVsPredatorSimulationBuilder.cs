namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Stages;
using System.Windows.Media.Imaging;

public class PreyVsPredatorSimulationBuilder(
    ISimulationBuilder simulationBuilder, 
    PreyVsPredatorDrawer drawer)
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        var simulation = simulationBuilder
            .CreateNewSimulation<PreyVsPredatorEnvironment>()
            .AddAgents<Prey>(100)
            .AddAgents<Predator>(100)
            .AddInitializationStage<ActorsPlaced>(c => c.SetSize(200, 200))
            .AddStage<PredatorsStarved>(c => c.ChangeStarvationIncrement(0.04))
            .AddStage<MovedPreyers>()
            .AddStage<MovedPredators>()
            .AddStage<BornNewPreyers>(c => c.ChangePregnancyUpdate(0.2))
            .AddStage<PreyersEaten>()
            .AddCallback(c => drawing(drawer.Draw(c)))
            .SetRandomSeed(100)
            .StopAgents()
            .Build();

        return simulation;
    }
}

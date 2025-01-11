namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation.Stages;
using System;
using System.Windows.Media.Imaging;

public class AntColonySimulationBuilder(ISimulationBuilder simulationBuilder, ColonyDrawer colonyDrawer)
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        var simulation = simulationBuilder
            .CreateNewSimulation<ColonyEnvironment>()
            .AddInitializationStage<SetColonySize>(s => s.SetSize(100, 100))
            .AddInitializationStage<AddAntHills>(s =>
            {
                s.AddAntHill(new AntHill { SizeRadius = 5, Coordinates = ColonyCoordinates.CreateAt(10, 10)});
            })
            .AddInitializationStage<AddFoodSources>(s =>
            {
                s.AddFoodSource(new FoodSource { SizeRadius = 5, Coordinates = ColonyCoordinates.CreateAt(50, 50) });
            })
            .AddAgents<Ant>(1000)
            .AddInitializationStage<SetAntsInRandomPositions>()
            .AddStage<ChangeAfterVisitHill>()
            .AddStage<ChangeAfterVisitFood>()
            .AddStage<DecreasePheromones>()
            .AddStage<AddFoodPheromones>()
            .AddStage<AddHomePheromones>()
            .AddStage<MoveAnts>()
            //.AddEnvironmentUpdates(DecreasePheromones)
            //.AddEnvironmentUpdates(AddPheromones)
            .AddCallback(c => drawing(colonyDrawer.Draw(c)))
            .SetRandomSeed(12557)
            .StopAgents()
            .Build();

        return simulation;
    }
}

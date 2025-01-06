namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Events;
using System.Windows.Media.Imaging;

public class PreyVsPredatorSimulationBuilder(
    ISimulationBuilder simulationBuilder, 
    PreyVsPredatorDrawer drawer)
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        var simulation = simulationBuilder
            .CreateNewSimulation<PreyVsPredatorEnvironment, PreyVsPredatorEnvironmentState>(
                PreyVsPredatorEnvironmentState.New(200, 200))
            .AddAgents<Prey>(100)
            .AddAgents<Predator>(100)
            .AddEnvironmentInitialization(async c =>
            {
                c.SimulationEnvironment.PlaceInitialPreys(c.GetAgents<Prey>().Select(x => x.Id).ToArray());
                c.SimulationEnvironment.PlaceInitialPredators(c.GetAgents<Predator>().Select(x => x.Id).ToArray());
            })
            .AddEvent<PredatorsStarved>(c => c.ChangeStarvationIncrement(0.04))
            .AddEvent<MovedPreyers>()
            .AddEvent<MovedPredators>()
            .AddEvent<BornNewPreyers>(c => c.ChangePregnancyUpdate(0.2))
            .AddEvent<PreyersEaten>()
            .AddCallback(c => drawing(drawer.Draw(c)))
            .SetRandomSeed(100)
            .Build();

        return simulation;
    }
}

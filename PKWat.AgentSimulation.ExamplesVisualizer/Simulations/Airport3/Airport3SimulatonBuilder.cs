using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.Airport3;
using PKWat.AgentSimulation.Examples.Airport3.Events;
using PKWat.AgentSimulation.Examples.Airport3.Stages;
using System.Windows.Media.Imaging;

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport3;

internal class Airport3SimulationBuilder(ISimulationBuilder simulationBuilder,
            AirportDrawer airportDrawer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        airportDrawer.InitializeIfNeeded(800, 800);

        var simulation = simulationBuilder
            .CreateNewSimulation<AirportEnvironment>()
            .AddInitializationStage<SetLandingLines>(s => s.SetMaxLandingLines(8))
            .AddInitializationEvent<NewAirplaneArrivedEvent>()
            .AddCallback(c => RenderAsync(c, drawing))
            .UseCalendar()
            .SetWaitingTimeBetweenSteps(TimeSpan.FromSeconds(0.1))
            .WithSnapshots()
            .Build();

        return simulation;
    }

    private async Task RenderAsync(ISimulationContext context, Action<BitmapSource> drawing)
            => drawing(airportDrawer.Draw(context));
}

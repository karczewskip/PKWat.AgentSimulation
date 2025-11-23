using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Examples.Airport2;
using PKWat.AgentSimulation.Examples.Airport2.Stages;
using System.Windows.Media.Imaging;

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport2;

internal class Airport2SimulationBuilder(ISimulationBuilder simulationBuilder,
            AirportDrawer airportDrawer) : IExampleSimulationBuilder
{
    public ISimulation Build(Action<BitmapSource> drawing)
    {
        airportDrawer.InitializeIfNeeded(800, 800);

        var simulation = simulationBuilder
            .CreateNewSimulation<AirportEnvironment>()
            .AddInitializationStage<SetLandingLines>(s => s.SetMaxLandingLines(8))
            .AddInitializationStage<NewAirplaneArrival>()
            .AddStage<NewAirplaneArrival>()
            .AddStage<AssignWaitingAirplanesToAvailableLines>()
            .AddStage<StartLandingAirplane>()
            .AddCallback(c => RenderAsync(c, drawing))
            .UseCalendar()
            .SetWaitingTimeBetweenSteps(TimeSpan.FromSeconds(1))
            .WithSnapshots()
            .Build();

        return simulation;
    }

    private async Task RenderAsync(ISimulationContext context, Action<BitmapSource> drawing)
            => drawing(airportDrawer.Draw(context));
}

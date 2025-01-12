namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport
{
    using PKWat.AgentSimulation.Core;
    using PKWat.AgentSimulation.Core.Builder;
    using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Stages;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    public class AirportSimulationBuilder : IExampleSimulationBuilder
    {
        private readonly ISimulationBuilder _simulationBuilder;
        private readonly AirportDrawer _airportDrawer;

        public AirportSimulationBuilder(
            ISimulationBuilder simulationBuilder,
            AirportDrawer airportDrawer)
        {
            _simulationBuilder = simulationBuilder;
            _airportDrawer = airportDrawer;
        }

        public ISimulation Build(Action<BitmapSource> drawing)
        {
            _airportDrawer.InitializeIfNeeded(800, 800);

            var simulation = _simulationBuilder
                .CreateNewSimulation<AirportEnvironment>()
                .AddInitializationStage<SetLindingLines>()
                .AddStage<NewAirplaneArrival>()
                .AddStage<ReleaseLines>()
                .AddStage<AssignWaitingAirplanesToAvailableLines>()
                .AddStage<StartLandingAirplane>()
                .AddStage<StartCheckoutingPassengers>()
                .AddStage<RemoveCheckoutedPassengers>()
                .AddStage<StartDeparture>()
                .AddCallback(c => RenderAsync(c, drawing))
                .SetSimulationStep(TimeSpan.FromMinutes(1))
                .SetWaitingTimeBetweenSteps(TimeSpan.FromSeconds(0.1))
                .SetRandomSeed(100)
                .WithSnapshots()
                .Build();

            return simulation;
        }

        private async Task RenderAsync(ISimulationContext context, Action<BitmapSource> drawing)
            => drawing(_airportDrawer.Draw(context));
    }
}

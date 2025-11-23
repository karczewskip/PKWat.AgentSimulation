namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport
{
    using PKWat.AgentSimulation.Core;
    using PKWat.AgentSimulation.Core.Builder;
    using PKWat.AgentSimulation.Core.Crash;
    using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Agents;
    using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport.Stages;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    public class AirportSimulationBuilder(
            ISimulationBuilder simulationBuilder,
            AirportDrawer airportDrawer) : IExampleSimulationBuilder

    {
        public ISimulation Build(Action<BitmapSource> drawing)
        {
            airportDrawer.InitializeIfNeeded(800, 800);

            var simulation = simulationBuilder
                .CreateNewSimulation<AirportEnvironment>()
                .AddInitializationStage<SetLindingLines>(s => s.SetMaxLandingLines(8))
                .AddStage<NewAirplaneArrival>()
                .AddStage<ReleaseLines>()
                .AddStage<AssignWaitingAirplanesToAvailableLines>()
                .AddStage<StartLandingAirplane>()
                .AddStage<StartCheckoutingPassengers>()
                .AddStage<RemoveCheckoutedPassengers>()
                .AddStage<StartDeparture>()
                .AddCrashCondition(c =>
                {
                    var airplanes = c.GetAgents<Airplane>();
                    var assignedLinesToAirplanes = airplanes
                        .Where(a => a.AssignedLine.HasValue)
                        .GroupBy(x => x.AssignedLine)
                        .Where(x => x.Count() > 1);

                    if (assignedLinesToAirplanes.Any())
                        return SimulationCrashResult.Crash("Two airplanes are assigned to the same line");

                    return SimulationCrashResult.NoCrash;

                })
                .AddCallback(c => RenderAsync(c, drawing))
                .SetSimulationStep(TimeSpan.FromMinutes(1))
                .SetWaitingTimeBetweenSteps(TimeSpan.FromSeconds(0.01))
                .SetRandomSeed(100)
                .WithSnapshots()
                .Build();

            return simulation;
        }

        private async Task RenderAsync(ISimulationContext context, Action<BitmapSource> drawing)
            => drawing(airportDrawer.Draw(context));
    }
}

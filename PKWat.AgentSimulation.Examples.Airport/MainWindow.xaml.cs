namespace PKWat.AgentSimulation.Examples.Airport
{
    using PKWat.AgentSimulation.Core;
    using System.Windows;
    using PKWat.AgentSimulation.Examples.Airport.Simulation;
    using PKWat.AgentSimulation.Examples.Airport.Simulation.Events;
    using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ISimulation _simulation;

        private readonly ISimulationBuilder _simulationBuilder;
        private readonly AirportDrawer _airportDrawer;

        public MainWindow(ISimulationBuilder simulationBuilder, AirportDrawer airportDrawer)
        {
            _simulationBuilder = simulationBuilder;

            InitializeComponent();
            _airportDrawer = airportDrawer;
            _airportDrawer.Initialize(800, 800);
        }

        private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_simulation?.Running ?? false)
            {
                await _simulation.StopAsync();
            }
            _simulation = _simulationBuilder
                .CreateNewSimulation(new AirportEnvironment())
                .AddAgent<Coordinator>()
                .AddEvent<NewAirplaneArrived>()
                .AddEnvironmentUpdates(UpdateAskingForLand)
                .AddEnvironmentUpdates(UpdateLandingAirplane)
                .AddEnvironmentUpdates(UpdateAllowedForLand)
                .AddCallback(RenderAsync)
                .SetSimulationStep(TimeSpan.FromMinutes(1))
                .SetWaitingTimeBetweenSteps(TimeSpan.FromSeconds(0.1))
                .SetRandomSeed(100)
                .Build();

            await _simulation.StartAsync();
        }

        private async Task UpdateAskingForLand(ISimulationContext<AirportEnvironment> context)
        {
            context
                .SimulationEnvironment
                .SetAirplanesAskingForLand(context
                    .GetAgents<Airplane>()
                    .Where(x => x.State.AskingForLand)
                    .Select(x => x.Id)
                    .ToArray());
        }

        private async Task UpdateLandingAirplane(ISimulationContext<AirportEnvironment> context)
        {
            context
                .SimulationEnvironment
                .SetLandingAirplane(context
                    .GetAgents<Airplane>()
                    .FirstOrDefault(x => x.State.IsLanding(context.SimulationTime.Time))
                    ?.Id ?? AgentId.Empty);
        }

        private async Task UpdateAllowedForLand(ISimulationContext<AirportEnvironment> context)
        {
            context
                .SimulationEnvironment
                .SetAllowedForLand(context
                    .GetRequiredAgent<Coordinator>()
                    .State
                    .AllowedAirplaneForLanding);
        }

        private async Task RenderAsync(ISimulationContext<AirportEnvironment> context)
            => simulationImage.Source = _airportDrawer.Draw(context);

        private async void stopSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_simulation?.Running ?? false)
            {
                await _simulation.StopAsync();
            }
        }
    }
}
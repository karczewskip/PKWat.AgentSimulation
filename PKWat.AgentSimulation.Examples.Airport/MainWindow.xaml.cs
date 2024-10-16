namespace PKWat.AgentSimulation.Examples.Airport
{
    using PKWat.AgentSimulation.Core;
    using PKWat.AgentSimulation.Examples.Airport.Simulation;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ISimulation _simulation;

        private readonly AirportSimulationBuilder _simulationBuilder;

        public MainWindow(AirportSimulationBuilder airportSimulationBuilder)
        {
            _simulationBuilder = airportSimulationBuilder;

            InitializeComponent();
        }

        private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_simulation?.Running ?? false)
            {
                await _simulation.StopAsync();
            }
            _simulation = _simulationBuilder.Build(bitmapSource => simulationImage.Source = bitmapSource);

            await _simulation.StartAsync();
        }

        private async void stopSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_simulation?.Running ?? false)
            {
                await _simulation.StopAsync();
            }
        }
    }
}
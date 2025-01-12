namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm
{
    using PKWat.AgentSimulation.Core;
    using PKWat.AgentSimulation.Core.Builder;
    using PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ISimulation _simulation;
        private int counter = 0;

        private readonly AntColonySimulationBuilder _simulationBuilder;

        public MainWindow(AntColonySimulationBuilder antColonySimulationBuilder)
        {
            _simulationBuilder = antColonySimulationBuilder;

            InitializeComponent();
        }

        private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_simulation?.Running ?? false)
            {
                await _simulation.StopAsync();
            }
            _simulation = _simulationBuilder.Build(bitmapSource =>
            {
                if (counter++ % 1 == 0)
                    Dispatcher.Invoke(() => simulationImage.Source = bitmapSource);
            });

            await Task.Run(async () => await _simulation.StartAsync());
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
using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Examples.GameOfLife.Simulation;
using System.Windows;

namespace PKWat.AgentSimulation.Examples.GameOfLife
{
    public partial class MainWindow : Window
    {
        private ISimulation _simulation;

        private readonly GameOfLifeSimulationBuilder _simulationBuilder;

        public MainWindow(GameOfLifeSimulationBuilder gameOfLifeSimulationBuilder)
        {
            _simulationBuilder = gameOfLifeSimulationBuilder;

            InitializeComponent();
        }

        private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_simulation?.Running ?? false)
            {
                await _simulation.StopAsync();
            }
            _simulation = _simulationBuilder.Build(bitmapSource => simulationImage.Source = bitmapSource, 100, 100);

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
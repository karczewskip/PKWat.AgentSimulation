using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Examples.GameOfLife.GPU.Simulation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PKWat.AgentSimulation.Examples.GameOfLife.GPU
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ISimulation _simulation;
        private int counter = 0;

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
            _simulation = _simulationBuilder.Build(bitmapSource =>
            {
                if(counter++ % 1 == 0)
                    simulationImage.Source = bitmapSource;
            }, 1000, 1000);

            _simulation.StartAsync();
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
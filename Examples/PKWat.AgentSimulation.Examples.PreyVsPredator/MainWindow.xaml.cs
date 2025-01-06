using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation;
using System.Diagnostics.Metrics;
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

namespace PKWat.AgentSimulation.Examples.PreyVsPredator;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ISimulation _simulation;

    private readonly PreyVsPredatorSimulationBuilder _simulationBuilder;

    public MainWindow(PreyVsPredatorSimulationBuilder simulationBuilder)
    {
        _simulationBuilder = simulationBuilder;

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
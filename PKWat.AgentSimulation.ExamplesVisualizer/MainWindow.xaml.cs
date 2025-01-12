namespace PKWat.AgentSimulation.ExamplesVisualizer;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations;
using System.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ISimulation simulation;
    private readonly SimulationsBuilder simulationsBuilder;

    public MainWindow(SimulationsBuilder simulationsBuilder)
    {
        this.simulationsBuilder = simulationsBuilder;

        InitializeComponent();
    }

    private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        if (simulation?.Running ?? false)
        {
            await simulation.StopAsync();
        }
        simulation = simulationsBuilder.BuildPreyVsPredatorSimulation(bitmapSource =>
        {
            Dispatcher.Invoke(() => simulationImage.Source = bitmapSource);
        });

        await Task.Run(async () => await simulation.StartAsync());
    }

    private async void stopSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        if (simulation?.Running ?? false)
        {
            await simulation.StopAsync();
        }
    }
}
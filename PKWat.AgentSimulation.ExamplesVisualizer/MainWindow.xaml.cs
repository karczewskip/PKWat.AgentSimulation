namespace PKWat.AgentSimulation.ExamplesVisualizer;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ISimulation simulation;
    private readonly Dictionary<string, IExampleSimulationBuilder> exampleSimulationBuilders;

    public MainWindow(
        IEnumerable<IExampleSimulationBuilder> exampleSimulationBuilders,
        ISimulationCyclePerformanceInfo performanceInfo)
    {
        this.exampleSimulationBuilders = exampleSimulationBuilders.ToDictionary(b => b.GetType().Name);

        InitializeComponent();

        foreach (var builderName in this.exampleSimulationBuilders.Keys)
        {
            simulationBuildersComboBox.Items.Add(builderName);
        }
        simulationBuildersComboBox.SelectedIndex = 0;

        performanceInfo.Subscribe(s => Dispatcher.Invoke(() => LogsTextBox.Text = s));
    }

    private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        if (simulation?.Running ?? false)
        {
            await simulation.StopAsync();
        }
        var selectedBuilder = simulationBuildersComboBox.SelectedValue.ToString();
        simulation = exampleSimulationBuilders[selectedBuilder].Build(bitmapSource =>
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
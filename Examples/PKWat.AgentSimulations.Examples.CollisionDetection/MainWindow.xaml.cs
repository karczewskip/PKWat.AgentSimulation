namespace PKWat.AgentSimulations.Examples.CollisionDetection;

using PKWat.AgentSimulation.Core;
using System.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const double ContainerWidth = 10000;
    private const double ContainerHeight = 10000;

    private readonly ISimulationBuilder _simulationBuilder;
    private readonly BallsContainerDrawer _ballsContainerDrawer;

    private ISimulation _simulation;


    public MainWindow(ISimulationBuilder simulationBuilder, BallsContainerDrawer ballsContainerDrawer)
    {
        _simulationBuilder = simulationBuilder;
        _ballsContainerDrawer = ballsContainerDrawer;

        _ballsContainerDrawer.Initialize(1000, 1000, ContainerWidth, ContainerHeight);

        InitializeComponent();
    }

    private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        if (_simulation?.Running ?? false)
        {
            await _simulation.StopAsync();
        }

        _simulation = _simulationBuilder
            .CreateNewSimulation(new BallsContainer(ContainerWidth, ContainerHeight, new BallAcceleration(0, -5)))
            .AddAgents<Ball>(10)
            .AddCallback(RenderAsync)
            .SetSimulationStep(TimeSpan.FromSeconds(0.5))
            .SetWaitingTimeBetweenSteps(TimeSpan.Zero)
            .Build();

        await _simulation.StartAsync();
    }

    private async void stopSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        if (_simulation?.Running ?? false)
        {
            await _simulation.StopAsync();
        }
    }

    private async Task RenderAsync(ISimulationContext<BallsContainer> context)
        => simulationImage.Source = _ballsContainerDrawer.Draw(context);
}
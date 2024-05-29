namespace PKWat.AgentSimulation.Examples.ButterflyEffect;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;
using System.Windows;

public partial class MainWindow : Window
{
    private ISimulation _simulation;

    private readonly ISimulationBuilder _simulationBuilder;
    private readonly ColorsGenerator _colorsGenerator;
    private readonly PictureRenderer _pictureRenderer;
    private readonly BouncingBallStateInitializer _bouncingBallStateInitializer;

    public MainWindow(ISimulationBuilder simulationBuilder, ColorsGenerator colorsGenerator, PictureRenderer pictureRenderer, BouncingBallStateInitializer bouncingBallStateInitializer)
    {
        _simulationBuilder = simulationBuilder;
        _colorsGenerator = colorsGenerator;
        _pictureRenderer = pictureRenderer;
        _bouncingBallStateInitializer = bouncingBallStateInitializer;

        InitializeComponent();
    }

    private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        double bulbRadius = 400.0;
        int ballsCount = 10;
        var random = new Random();

        var colors = _colorsGenerator.Generate(ballsCount);

        _bouncingBallStateInitializer.Initialize(colors, ballsCount);
        var bitmapSize = (int)(bulbRadius * 2);
        _pictureRenderer.Initialize(bitmapSize, bitmapSize);

        var environment = new BouncingBallBulb(bulbRadius, 10.0);

        _simulation
            = _simulationBuilder
                .CreateNewSimulation(environment)
                .AddAgents<BouncingBall, BouncingBallState>(200, randomNumbersGenerator =>
                {
                    return _bouncingBallStateInitializer.InitializeNewState(environment);
                })
                .AddCallback(RenderAsync)
                .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(10))
                .Build();

        await _simulation.StartAsync();
    }

    private async void stopSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        await _simulation.StopAsync();
    }

    private async Task RenderAsync(ISimulationContext<BouncingBallBulb> simulationContext)
    {
        simulationImage.Source = _pictureRenderer.Draw(simulationContext);
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        //_centerTransform = new TranslateTransform()
        //{
        //    X = simulationCanvas.ActualWidth / 2,
        //    Y = simulationCanvas.ActualHeight / 2
        //};
    }
}
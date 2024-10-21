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
        int ballsCount = 200;

        var colors = _colorsGenerator.Generate(ballsCount);

        _bouncingBallStateInitializer.Initialize(colors, ballsCount);
        var bitmapSize = (int)(bulbRadius * 2);
        _pictureRenderer.Initialize(bitmapSize, bitmapSize);

        _simulation
            = _simulationBuilder
                .CreateNewSimulation<BouncingBallBulb, BouncingBallBulbState>(new BouncingBallBulbState(bulbRadius, 10.0, 0.25))
                .AddAgents<BouncingBall>(ballsCount)
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
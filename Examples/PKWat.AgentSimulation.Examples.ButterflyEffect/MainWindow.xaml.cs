namespace PKWat.AgentSimulation.Examples.ButterflyEffect;

using PKWat.AgentSimulation.Core;
using System.Diagnostics.Metrics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

public partial class MainWindow : Window
{
    private const int _radius = 400;

    private TranslateTransform _centerTransform;

    private ISimulation _simulation;

    private readonly ISimulationBuilder _simulationBuilder;
    private readonly ColorsGenerator _colorsGenerator;
    private readonly PictureRenderer _pictureRenderer;

    public MainWindow(ISimulationBuilder simulationBuilder, ColorsGenerator colorsGenerator, PictureRenderer pictureRenderer)
    {
        _simulationBuilder = simulationBuilder;
        _colorsGenerator = colorsGenerator;
        _pictureRenderer = pictureRenderer;

        InitializeComponent();
    }

    private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        int ballsCount = 500;
        var random = new Random();

        var colors = _colorsGenerator.Generate(ballsCount);
        var bouncingBalls = Enumerable.Range(0, ballsCount).Select(x =>
        {
            var color = colors[x];
            var startX = ((ballsCount / 2 - x) * 0.00001) / ballsCount;
            var radius = (1 + random.NextDouble())*10;
            var startY = -(_radius/2);
            var startDeltaX = 0;
            var startDeltaY = 0;
            var maxDistanceFromTheCenter = _radius;
            var gravity = 0.25;
            var brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            return new BouncingBall(startX, startY, radius, startDeltaX, startDeltaY, maxDistanceFromTheCenter, gravity, brush);
        }).ToArray();

        _simulation
            = _simulationBuilder
                .CreateNewSimulation()
                .AddAgents(bouncingBalls)
                .AddCallback(RenderAsync)
                .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(10))
                .Build();

        await _simulation.StartAsync();
    }

    private async void stopSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        await _simulation.StopAsync();
    }

    private async Task RenderAsync(ISimulationContext simulationContext)
    {
        var bouncingBalls = simulationContext.GetAgents<BouncingBall>().ToArray();

        simulationImage.Source = _pictureRenderer.Render(_radius, bouncingBalls);
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
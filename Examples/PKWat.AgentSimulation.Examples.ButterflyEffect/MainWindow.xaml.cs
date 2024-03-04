namespace PKWat.AgentSimulation.Examples.ButterflyEffect;

using PKWat.AgentSimulation.Core;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

public partial class MainWindow : Window
{
    private const int _radius = 300;
    private Stopwatch _stopwatch;

    private TranslateTransform _centerTransform;
    private BouncingBall[] _bouncingBalls;

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
        if(_simulation != null)
        {
            await _simulation.StopAsync();
        }

        int ballsCount = 20000;
        var random = new Random();

        var startDeltaX = random.Next(10);
        var startDeltaY = random.Next(10);
        var maxDistanceFromTheCenter = _radius;
        var gravity = 5 * random.NextDouble();

        var colors = _colorsGenerator.Generate(ballsCount);
        _bouncingBalls = Enumerable.Range(0, ballsCount).Select(x =>
        {
            var color = colors[x];
            var startX = ((ballsCount / 2 - x) * 0.00001) / ballsCount;
            var radius = 10;// +(1 + random.NextDouble())*8;
            var startY = -(_radius/2);
            var brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            return new BouncingBall(startX, startY, radius, startDeltaX, startDeltaY, maxDistanceFromTheCenter, gravity, brush);
        }).OrderBy(x => Guid.NewGuid()).ToArray();

        _pictureRenderer.Initialize(_radius);

        _simulation
            = _simulationBuilder
                .CreateNewSimulation()
                .AddAgents(_bouncingBalls)
                .AddCallback(RenderAsync)
                .SetWaitingTimeBetweenSteps(TimeSpan.Zero)
                .Build();

        _stopwatch = Stopwatch.StartNew();

        await _simulation.StartAsync();
    }

    private async void stopSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        if (_simulation != null)
        {
            await _simulation.StopAsync();
        }
    }

    private async Task RenderAsync()
    {
        _stopwatch.Stop();
        Debug.WriteLine("Fps: " + 1000 / _stopwatch.ElapsedMilliseconds);
        _stopwatch.Restart();
        simulationImage.Source = _pictureRenderer.Render(_bouncingBalls);
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
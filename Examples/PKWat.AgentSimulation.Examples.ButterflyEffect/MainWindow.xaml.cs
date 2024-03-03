namespace PKWat.AgentSimulation.Examples.ButterflyEffect;

using PKWat.AgentSimulation.Core;
using System.Diagnostics.Metrics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

public partial class MainWindow : Window
{
    private const int _radius = 400;
    private int counter = 0;

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
        //_centerTransform = new TranslateTransform()
        //{
        //    X = simulationCanvas.ActualWidth / 2,
        //    Y = simulationCanvas.ActualHeight / 2
        //};
        //simulationCanvas.Background = Brushes.Black;
        //var bitmapImage = new BitmapImage();
        //bitmapImage.SetValue()
        //simulationImage.Source = new BitmapImage();

        //var bitmap = new Bitmap()

        int ballsCount = 1000;

        var colors = _colorsGenerator.Generate(ballsCount);
        _bouncingBalls = Enumerable.Range(0, ballsCount).Select(x =>
        {
            var color = colors[x];
            var startX = ((ballsCount / 2 - x) * 0.00001) / ballsCount;
            var radius = 10;
            var startY = -(_radius - radius);
            var startDeltaX = 0;
            var startDeltaY = 0;
            var maxDistanceFromTheCenter = _radius;
            var gravity = 1;
            var brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            return new BouncingBall(startX, startY, radius, startDeltaX, startDeltaY, maxDistanceFromTheCenter, gravity, brush);
        }).ToArray();

        _simulation
            = _simulationBuilder
                .CreateNewSimulation()
                .AddAgents(_bouncingBalls)
                .AddCallback(RenderAsync)
                .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(16))
                .Build();

        await _simulation.StartAsync();
    }

    private async void stopSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        await _simulation.StopAsync();
    }

    private async Task RenderAsync()
    {
        simulationImage.Source = _pictureRenderer.Render(_radius, _bouncingBalls);
        //counter = (counter + 1) % 4;
        //if(counter == 0)
        //{
        //    simulationImage.Source = _pictureRenderer.Render(_radius, _bouncingBalls);
        //}
        //await Application.Current.Dispatcher.InvokeAsync(() =>
        //{
        //    var circle = CreateCircle(new Point(0,0) ,_radius, Brushes.White);
        //    var balls = _bouncingBalls.Select(x => CreateCircle(new Point(x.X, x.Y), x.Radius, x.Brush));
        //    var center = CreateCircle(new Point(0, 0), 10, Brushes.Black);

        //    simulationCanvas.Children.Clear();

        //    simulationCanvas.Children.Add(circle);
        //    foreach (var b in balls)
        //    {
        //        simulationCanvas.Children.Add(b);
        //    }
        //    //simulationCanvas.Children.Add(center);
        //});
    }

    private Path CreateCircle(Point center, double radius, Brush brush)
    {
        var circle = new Path();
        var circleGeometry = new PathGeometry();
        var ellipseGeometry = new EllipseGeometry();
        ellipseGeometry.Center = center;
        ellipseGeometry.RadiusX = radius;
        ellipseGeometry.RadiusY = radius;
        circleGeometry.AddGeometry(ellipseGeometry);

        circle.Data = circleGeometry;
        circle.Fill = brush;
        circle.RenderTransform = _centerTransform;

        return circle;
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
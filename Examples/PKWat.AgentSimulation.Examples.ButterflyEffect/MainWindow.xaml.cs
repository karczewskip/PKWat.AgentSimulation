namespace PKWat.AgentSimulation.Examples.ButterflyEffect;

using PKWat.AgentSimulation.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

public partial class MainWindow : Window
{
    private const int _radius = 400;

    private TranslateTransform _centerTransform;
    private BouncingBall _ball;

    private readonly ISimulationBuilder _simulationBuilder;

    public MainWindow(ISimulationBuilder simulationBuilder)
    {
        _simulationBuilder = simulationBuilder;

        InitializeComponent();
    }

    private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        _centerTransform = new TranslateTransform()
        {
            X = simulationCanvas.ActualWidth / 2,
            Y = simulationCanvas.ActualHeight / 2
        };
        simulationCanvas.Background = Brushes.Black;

        _ball = new BouncingBall(_radius);

        var simulation
            = _simulationBuilder
                .CreateNewSimulation()
                .AddAgent(_ball)
                .AddCallback(() => RenderAsync())
                .Build();

        await simulation.StartAsync();
    }

    private async Task RenderAsync()
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var circle = CreateCircle(new Point(0,0) ,_radius, Brushes.White);
            var ball = CreateCircle(new Point(_ball.X, _ball.Y), _ball.Radius, Brushes.Blue);

            simulationCanvas.Children.Clear();

            simulationCanvas.Children.Add(circle);
            simulationCanvas.Children.Add(ball);
        });
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
        _centerTransform = new TranslateTransform()
        {
            X = simulationCanvas.ActualWidth / 2,
            Y = simulationCanvas.ActualHeight / 2
        };
    }

    private class BouncingBall : IAgent
    {
        private readonly double _maxDistanceFromCenter;

        public BouncingBall(double maxDistanceFromCenter)
        {
            _maxDistanceFromCenter = maxDistanceFromCenter;
        }

        public double X { get; private set; } = 0;
        public double Y { get; private set; } = 0;

        public double Radius { get; private set; } = 10;

        public void Act()
        {
            if(_maxDistanceFromCenter > Math.Sqrt(X*X + Y * Y) + Radius)
            {
                Y += 1;
            }
        }
    }

    private class MovingRectangleAgent : IAgent
    {
        private int _counter = 0;
        private readonly Canvas _canvas;

        public MovingRectangleAgent(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void Act()
        {
            _counter = (_counter + 1) % 100;
            Application.Current.Dispatcher.Invoke(() => {
                var movingRectangle = new Polygon();
                movingRectangle.Points.Add(new Point(_counter, _counter));
                movingRectangle.Points.Add(new Point(_counter + 10, _counter));
                movingRectangle.Points.Add(new Point(_counter + 10, _counter + 10));
                movingRectangle.Points.Add(new Point(_counter, _counter + 10));
                movingRectangle.Fill = Brushes.Blue;

                _canvas.Children.Clear();

                _canvas.Children.Add(movingRectangle);
            });
        }
    }
}
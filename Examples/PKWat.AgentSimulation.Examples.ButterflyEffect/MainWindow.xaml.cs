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
    private BouncingBall[] _bouncingBalls;

    private ISimulation _simulation;

    private readonly ISimulationBuilder _simulationBuilder;
    private readonly ColorsGenerator _colorsGenerator;

    public MainWindow(ISimulationBuilder simulationBuilder, ColorsGenerator colorsGenerator)
    {
        _simulationBuilder = simulationBuilder;
        _colorsGenerator = colorsGenerator;

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

        int ballsCount = 100;

        var colors = _colorsGenerator.Generate(ballsCount);
        _bouncingBalls = Enumerable.Range(1, ballsCount).Select(x =>
        {
            var color = colors[x - 1];
            return new BouncingBall(((ballsCount / 2 - x)*0.001) / ballsCount, -_radius/2, 10, 0, 0, _radius, 1, new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B)));
        }).ToArray();

        _simulation
            = _simulationBuilder
                .CreateNewSimulation()
                .AddAgents(_bouncingBalls)
                .AddCallback(RenderAsync)
                .Build();

        await _simulation.StartAsync();
    }

    private async void stopSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        await _simulation.StopAsync();
    }

    private async Task RenderAsync()
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var circle = CreateCircle(new Point(0,0) ,_radius, Brushes.White);
            var balls = _bouncingBalls.Select(x => CreateCircle(new Point(x.X, x.Y), x.Radius, x.Brush));
            var center = CreateCircle(new Point(0, 0), 10, Brushes.Black);

            simulationCanvas.Children.Clear();

            simulationCanvas.Children.Add(circle);
            foreach (var b in balls)
            {
                simulationCanvas.Children.Add(b);
            }
            //simulationCanvas.Children.Add(center);
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
        private readonly double _gravity;

        public BouncingBall(
            double startX,
            double startY,
            double radius,
            double startDeltaX,
            double startDeltaY,
            double maxDistanceFromCenter,
            double gravity,
            Brush brush)
        {
            X = startX;
            Y = startY;
            Radius = radius;
            DeltaX = startDeltaX;
            DeltaY = startDeltaY;
            Brush = brush;
            _maxDistanceFromCenter = maxDistanceFromCenter;
            _gravity = gravity;
        }

        public double X { get; private set; } = 0;
        public double Y { get; private set; } = 0;

        public double DeltaX { get; private set; } = 0;
        public double DeltaY { get; private set; } = 1;

        public double Radius { get; private set; } = 10;
        public Brush Brush { get; }

        public double VelocityRadian => Math.Atan2(DeltaY, DeltaX);
        public double VelocityDirection => Math.Tan(VelocityRadian);
        public double NormalRadian => Math.Atan2(Y, X);
        public double NormalDirection => Math.Tan(NormalRadian);

        public void Act()
        {
            DeltaY += _gravity;
            var newX = X + DeltaX;
            var newY = Y + DeltaY;
            if(_maxDistanceFromCenter > Math.Sqrt(newX * newX + newY * newY) + Radius)
            {
                X = newX;
                Y = newY;
            }
            else
            {
                double newDirectionRadian = 2 * NormalRadian - VelocityRadian;
                double speed = Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);
                DeltaX = - Math.Cos(newDirectionRadian) * speed;
                DeltaY = - Math.Sin(newDirectionRadian) * speed;
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
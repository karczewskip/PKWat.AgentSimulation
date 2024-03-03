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

    private ISimulation _simulation;

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

        _ball = new BouncingBall(100, 0, 1, 2, _radius);

        _simulation
            = _simulationBuilder
                .CreateNewSimulation()
                .AddAgent(_ball)
                .AddCallback(() => RenderAsync())
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
            var ball = CreateCircle(new Point(_ball.X, _ball.Y), _ball.Radius, Brushes.Blue);
            var center = CreateCircle(new Point(0, 0), 10, Brushes.Black);
            var ballVelocity = new Line()
            {
                X1 = -_radius,
                Y1 = _ball.VelocityDirection * (-_radius - _ball.X) + _ball.Y,
                X2 = _radius,
                Y2 = _ball.VelocityDirection * (_radius - _ball.X) + _ball.Y,
                Stroke = Brushes.Green,
                StrokeThickness = 1,
                RenderTransform = _centerTransform
            };
            var normal = new Line()
            {
                X1 = -_radius,
                Y1 = _ball.NormalDirection * (-_radius),
                X2 = _radius,
                Y2 = _ball.NormalDirection * _radius,
                Stroke = Brushes.Blue,
                StrokeThickness = 1,
                RenderTransform = _centerTransform
            };
            var newDirection = Math.Tan(2* _ball.NormalRadian - _ball.VelocityRadian);
            var newDirectionLine = new Line()
            {
                X1 = -_radius,
                Y1 = newDirection * (-_radius - _ball.X) + _ball.Y,
                X2 = _radius,
                Y2 = newDirection * (_radius - _ball.X) + _ball.Y,
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                RenderTransform = _centerTransform
            };
            //var tangentDirection = -1 / normalDirection;
            //var startTangentX = _ball.X - 50;
            //var endTangentX = _ball.X + 50;
            //var tangent = new Line()
            //{
            //    X1 = startTangentX,
            //    Y1 = tangentDirection * (startTangentX - _ball.X) + _ball.Y,
            //    X2 = endTangentX,
            //    Y2 = tangentDirection * (endTangentX - _ball.X) + _ball.Y,
            //    Stroke = Brushes.Red,
            //    StrokeThickness = 1,
            //    RenderTransform = _centerTransform
            //};

            simulationCanvas.Children.Clear();

            simulationCanvas.Children.Add(circle);
            simulationCanvas.Children.Add(ball);
            simulationCanvas.Children.Add(center);
            simulationCanvas.Children.Add(ballVelocity);
            simulationCanvas.Children.Add(normal);
            simulationCanvas.Children.Add(newDirectionLine);
            //simulationCanvas.Children.Add(tangent);
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

        public BouncingBall(
            double startX,
            double startY,
            double startDeltaX,
            double startDeltaY,
            double maxDistanceFromCenter)
        {
            X = startX;
            Y = startY;
            DeltaX = startDeltaX;
            DeltaY = startDeltaY;
            _maxDistanceFromCenter = maxDistanceFromCenter;
        }

        public double X { get; private set; } = 0;
        public double Y { get; private set; } = 0;

        public double DeltaX { get; private set; } = 0;
        public double DeltaY { get; private set; } = 1;

        public double Radius { get; private set; } = 10;

        public double VelocityRadian => Math.Atan2(DeltaY, DeltaX);
        public double VelocityDirection => Math.Tan(VelocityRadian);
        public double NormalRadian => Math.Atan2(Y, X);
        public double NormalDirection => Math.Tan(NormalRadian);

        public void Act()
        {
            var newX = X + DeltaX;
            var newY = Y + DeltaY;
            if(_maxDistanceFromCenter > Math.Sqrt(newX * newX + newY * newY) + Radius)
            {
                X = newX;
                Y = newY;
            }
            else
            {
                //double bouncingRadian = velocityRadian - normalRadian;
                //double newDirectionRadian = normalRadian - bouncingRadian;
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
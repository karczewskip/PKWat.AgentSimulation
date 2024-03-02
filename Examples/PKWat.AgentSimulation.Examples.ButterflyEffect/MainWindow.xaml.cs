namespace PKWat.AgentSimulation.Examples.ButterflyEffect;

using PKWat.AgentSimulation.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

public partial class MainWindow : Window
{
    private readonly ISimulationBuilder _simulationBuilder;

    public MainWindow(ISimulationBuilder simulationBuilder)
    {
        _simulationBuilder = simulationBuilder;

        InitializeComponent();
    }

    private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        var simulation 
            = _simulationBuilder
                .CreateNewSimulation()
                .AddAgent(new MovingRectangleAgent(simulationCanvas))
                .Build();

        await simulation.StartAsync();
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
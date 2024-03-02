namespace PKWat.AgentSimulation.Examples.ButterflyEffect;

using PKWat.AgentSimulation.Core;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

public partial class MainWindow : Window
{
    private readonly ISimulation _simulation;

    public MainWindow(ISimulation simulation)
    {
        _simulation = simulation;

        InitializeComponent();
    }

    private void startSimulationButton_Click(object sender, RoutedEventArgs e)
    {
        _simulation.Start(() =>
        {
            var examplePolygon = new Polygon();
            examplePolygon.Points.Add(new Point(10, 10));
            examplePolygon.Points.Add(new Point(20, 10));
            examplePolygon.Points.Add(new Point(20, 20));
            examplePolygon.Points.Add(new Point(10, 20));
            examplePolygon.Fill = Brushes.Blue;

            simulationCanvas.Children.Add(examplePolygon);
        });
    }
}
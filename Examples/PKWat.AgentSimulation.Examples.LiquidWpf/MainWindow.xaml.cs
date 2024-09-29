namespace PKWat.AgentSimulation.Examples.LiquidWpf
{
    using PKWat.AgentSimulation.Core;
    using PKWat.AgentSimulation.Examples.LiquidWpf.Simulation;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ISimulation _simulation;

        private readonly LiquidSimulation _liquidSimulation;

        public MainWindow(LiquidSimulation liquidSimulation)
        {
            _liquidSimulation = liquidSimulation;

            InitializeComponent();
        }

        private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            _simulation = _liquidSimulation.CreateSimulation(b => simulationImage.Source = b);

            await _simulation.StartAsync();
        }

        private async void stopSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            await _simulation.StopAsync();
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
}
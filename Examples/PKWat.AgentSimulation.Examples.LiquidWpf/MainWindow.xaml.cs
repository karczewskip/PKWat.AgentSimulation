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

        private readonly ISimulationBuilder _simulationBuilder;
        private readonly LiquidRenderer _liquidRenderer;

        public MainWindow(ISimulationBuilder simulationBuilder, LiquidRenderer liquidRenderer)
        {
            _simulationBuilder = simulationBuilder;
            _liquidRenderer = liquidRenderer;

            InitializeComponent();
        }

        private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
        {

            _liquidRenderer.Initialize(800, 600);

            _simulation
                = _simulationBuilder
                    .CreateNewSimulation(new BinEnvironment(1000, 1000))
                    .AddAgent<Drop>()
                    .AddCallback(RenderAsync)
                    .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(1000))
                    .Build();

            await _simulation.StartAsync();
        }

        private async void stopSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            await _simulation.StopAsync();
        }

        private async Task RenderAsync(ISimulationContext<BinEnvironment> simulationContext)
        {
            simulationImage.Source = _liquidRenderer.Draw(simulationContext);
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
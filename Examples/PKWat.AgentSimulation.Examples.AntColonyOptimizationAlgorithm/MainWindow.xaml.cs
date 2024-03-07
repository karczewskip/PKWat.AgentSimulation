namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm
{
    using PKWat.AgentSimulation.Core;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ISimulation _simulation;

        private readonly ISimulationBuilder _simulationBuilder;
        private readonly ColonyDrawer _colonyDrawer;

        public MainWindow(ISimulationBuilder simulationBuilder, ColonyDrawer colonyDrawer)
        {
            _simulationBuilder = simulationBuilder;
            _colonyDrawer = colonyDrawer;
            _colonyDrawer.Initialize(600, 600);

            InitializeComponent();
        }

        private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            if(_simulation?.Running ?? false)
            {
                await _simulation.StopAsync();
            }

            _simulation = _simulationBuilder
                .CreateNewSimulation(new ColonyEnvironment(500, 500, new AntHill(new ColonyCoordinates(100, 100))))
                .AddAgents(Enumerable.Range(0, 10).Select(x => new Ant(new ColonyCoordinates(x * 10, x * 20))).ToArray())
                .AddCallback(RenderAsync)
                .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(10))
                .Build();

            await _simulation.StartAsync();
        }

        private async Task RenderAsync(ISimulationContext<ColonyEnvironment> context) 
            => simulationImage.Source = _colonyDrawer.Draw(context);

        private async void stopSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_simulation?.Running ?? false)
            {
                await _simulation.StopAsync();
            }
        }
    }
}
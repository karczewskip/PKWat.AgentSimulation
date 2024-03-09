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
        private const int Scale = 2;
        private ISimulation _simulation;

        private readonly ISimulationBuilder _simulationBuilder;
        private readonly ColonyDrawer _colonyDrawer;

        public MainWindow(ISimulationBuilder simulationBuilder, ColonyDrawer colonyDrawer)
        {
            _simulationBuilder = simulationBuilder;
            _colonyDrawer = colonyDrawer;
            _colonyDrawer.Initialize(Scale * 60, Scale * 60);

            InitializeComponent();
        }

        private async void startSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            if(_simulation?.Running ?? false)
            {
                await _simulation.StopAsync();
            }

            _simulation = _simulationBuilder
                .CreateNewSimulation(new ColonyEnvironment(Scale* 50, Scale*50, new AntHill(new ColonyCoordinates(Scale*10, Scale * 10)), new FoodSource(new ColonyCoordinates(Scale* 40, Scale * 40))))
                .AddAgents(Enumerable.Range(0, 200).Select(x => new Ant()).ToArray())
                .AddEnvironmentUpdates(DecreasePheromones)
                .AddCallback(RenderAsync)
                .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(1))
                .Build();

            await _simulation.StartAsync();
        }

        private async Task DecreasePheromones(ISimulationContext<ColonyEnvironment> context)
        {
            context.SimulationEnvironment.DecreasePheromones();
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
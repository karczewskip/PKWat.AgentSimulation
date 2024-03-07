namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm
{
    using System.Text;
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
        private readonly ColonyDrawer _colonyDrawer;

        public MainWindow(ColonyDrawer colonyDrawer)
        {
            _colonyDrawer = colonyDrawer;

            InitializeComponent();
        }

        private void startSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            var ants = Enumerable.Range(0, 10).Select(x => new Ant(x * 10, x * 20)).ToArray();
            _colonyDrawer.Initialize(500, 500);

            simulationImage.Source = _colonyDrawer.Draw(ants);
        }

        private void stopSimulationButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
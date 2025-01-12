namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations;

using PKWat.AgentSimulation.Core;
using System.Windows.Media.Imaging;

public interface IVisualizationDrawer
{
    BitmapSource Draw(ISimulationContext context);
}

namespace PKWat.AgentSimulation.Examples.Airport.Simulation;

using PKWat.AgentSimulation.Core;
using System.Drawing;
using System.Windows.Media.Imaging;
using PKWat.AgentSimulation.Drawing;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

public class AirportDrawer
{
    private Bitmap _bmp;

    public void Initialize(int width, int height)
    {
        _bmp = new Bitmap(width, height);
        _bmp.SetResolution(96, 96);
    }

    public BitmapSource Draw(ISimulationContext<AirportEnvironment> context)
    {
        using var graphic = Graphics.FromImage(_bmp);
        graphic.Clear(Color.White);

        int i = 0;
        foreach (var airplane in context.GetAgents<Airplane>())
        {
            graphic.FillEllipse(Brushes.Black, _bmp.Width/2 + i*5, _bmp.Height/2, 5, 5);
            i++;
        }

        return _bmp.ConvertToBitmapSource();
    }
}

namespace PKWat.AgentSimulation.Examples.Airport.Simulation;

using PKWat.AgentSimulation.Core;
using System.Drawing;
using System.Windows.Media.Imaging;
using PKWat.AgentSimulation.Drawing;

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

        return _bmp.ConvertToBitmapSource();
    }
}

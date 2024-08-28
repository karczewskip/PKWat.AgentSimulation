namespace PKWat.AgentSimulation.Examples.LiquidWpf.Simulation;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;
using System.Drawing;
using System.Windows.Media.Imaging;

public class LiquidRenderer
{
    private const int Scale = 1;

    private Bitmap _bmp;

    public void Initialize(int width, int height)
    {
        _bmp = new Bitmap(Scale * width, Scale * height);
        _bmp.SetResolution(96, 96);
    }

    public BitmapSource Draw(ISimulationContext<BinEnvironment> context)
    {
        using var graphic = Graphics.FromImage(_bmp);
        graphic.Clear(Color.Black);


        return _bmp.ConvertToBitmapSource();
    }
}

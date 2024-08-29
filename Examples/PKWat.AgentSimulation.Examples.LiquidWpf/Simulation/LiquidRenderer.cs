namespace PKWat.AgentSimulation.Examples.LiquidWpf.Simulation;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;
using PKWat.AgentSimulation.Math.Extensions;
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

        var pen = new Pen(Brushes.White, 0);
        var drops = context.GetAgents<Drop>();
        foreach(var drop in drops)
        {
            graphic.DrawEllipse(
                pen, 
                (float)drop.State.Position.X.ScaleToView(context.SimulationEnvironment.BinWidth, _bmp.Width), 
                (float)drop.State.Position.Y.ScaleToView(context.SimulationEnvironment.BinHeight, _bmp.Height), 
                5, 
                5);
        }


        return _bmp.ConvertToBitmapSource();
    }
}

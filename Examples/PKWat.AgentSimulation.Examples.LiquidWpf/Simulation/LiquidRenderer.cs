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
        using var graphics = Graphics.FromImage(_bmp);
        graphics.Clear(Color.Black);

        DrawLiquid(context, graphics);
        DrawHeatmap(context, graphics);

        return _bmp.ConvertToBitmapSource();
    }

    private void DrawLiquid(ISimulationContext<BinEnvironment> context, Graphics graphics)
    {
        var pen = new Pen(Brushes.White, 0);
        var drops = context.GetAgents<Drop>();
        var width = _bmp.Width / 2;
        foreach (var drop in drops)
        {
            graphics.DrawEllipse(
                pen,
                (float)drop.State.Position.X.ScaleToView(context.SimulationEnvironment.BinWidth, width),
                (float)drop.State.Position.Y.ScaleToView(context.SimulationEnvironment.BinHeight, _bmp.Height),
                5,
                5);
        }
    }

    private void DrawHeatmap(ISimulationContext<BinEnvironment> context, Graphics graphics)
    {
        var environment = context.SimulationEnvironment;
        var heatmap = HeatMap.Create(
            environment.BinWidth, 
            environment.BinHeight, 
            100, 
            100, 
            context.GetAgents<Drop>().Select(x => (x.State.Position.X, x.State.Position.Y)).ToArray());
        var width = _bmp.Width / 2;
        var height = _bmp.Height;
        var coldColor = Color.FromArgb(255, 0, 0, 255);
        var hotColor = Color.FromArgb(255, 255, 0, 0);

        graphics.FillRectangle(
            new SolidBrush(coldColor),
            width,
            0,
            width,
            height);

        foreach(var heatmapValue in heatmap.HeatmapValues)
        {
            var value = heatmapValue.Counter;
            var valueNormalized = value / 10.0;

            var color = Color.FromArgb(
                255,
                (int)valueNormalized.Lerp(coldColor.R, hotColor.R),
                (int)valueNormalized.Lerp(coldColor.G, hotColor.G),
                (int)valueNormalized.Lerp(coldColor.B, hotColor.B));

            graphics.FillRectangle(
                new SolidBrush(color),
                (float)heatmapValue.X.ScaleToView(context.SimulationEnvironment.BinWidth, width, width),
                (float)heatmapValue.Y.ScaleToView(context.SimulationEnvironment.BinHeight, height),
                (float)heatmap.CellWidth,
                (float)heatmap.CellHeight);
        }
    }
}

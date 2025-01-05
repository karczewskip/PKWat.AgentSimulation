namespace PKWat.AgentSimulation.Examples.GameOfLife.GPU.Simulation;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using System.Drawing;
using System.Windows.Media.Imaging;

public class GameOfLifeDrawer(ISimulationCyclePerformanceInfo performanceInfoProvider)
{
    public BitmapSource Draw(ISimulationContext<LifeMatrixEnvironment> context)
    {
        var width = context.SimulationEnvironment.GetWidth();
        var height = context.SimulationEnvironment.GetHeight();
        var stride = width * 4;

        var writableBitmap = new WriteableBitmap(width, height, 96, 96, System.Windows.Media.PixelFormats.Bgr32, null);
        var pixels = new byte[width * height * 4];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var index = (i * height + j) * 4;
                if (context.SimulationEnvironment.IsCellAlive(i, j))
                {
                    pixels[index] = 0;
                    pixels[index + 1] = 0;
                    pixels[index + 2] = 0;
                    pixels[index + 3] = 255;
                }
                else
                {
                    pixels[index] = 255;
                    pixels[index + 1] = 255;
                    pixels[index + 2] = 255;
                    pixels[index + 3] = 255;
                }
            }
        }
        writableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixels, width * 4, 0);

        var pixelPtr = writableBitmap.BackBuffer;
        var bitmap = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppRgb, pixelPtr);

        writableBitmap.Lock();

        using var graphic = Graphics.FromImage(bitmap);

        graphic.DrawString(performanceInfoProvider.GetPerformanceInfo(), new Font("Arial", 8), Brushes.Red, 0, 0);

        writableBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, width, height));
        writableBitmap.Unlock();

        return writableBitmap;
    }

}

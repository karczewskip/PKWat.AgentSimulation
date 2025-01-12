namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using System.Drawing;
using System.Windows.Media.Imaging;

public class PreyVsPredatorDrawer(ISimulationCyclePerformanceInfo performanceInfoProvider)
{
    public BitmapSource Draw(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<PreyVsPredatorEnvironment>();

        var width = environment.GetWidth();
        var height = environment.GetHeight();
        var wholeImageSize = (int)(height * 1.5);
        var stride = width * 4;

        var writableBitmap = new WriteableBitmap(width, wholeImageSize, 96, 96, System.Windows.Media.PixelFormats.Bgr32, null);

        var pixels = new byte[width * wholeImageSize * 4];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var index = (i * height + j) * 4;
                if (environment.IsPredatorAt(i, j))
                {
                    pixels[index] = 255;
                    pixels[index + 1] = 0;
                    pixels[index + 2] = 0;
                    pixels[index + 3] = 255;
                }
                else if(environment.IsPreyAt(i, j))
                {
                    pixels[index] = 0;
                    pixels[index + 1] += 255;
                    pixels[index + 2] += 0;
                    pixels[index + 3] += 255;
                }
            }
        }
        writableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, wholeImageSize), pixels, width * 4, 0);

        var pixelPtr = writableBitmap.BackBuffer;
        var bitmap = new Bitmap(width, wholeImageSize, stride, System.Drawing.Imaging.PixelFormat.Format32bppRgb, pixelPtr);

        writableBitmap.Lock();

        using var graphic = Graphics.FromImage(bitmap);

        graphic.DrawString(performanceInfoProvider.GetPerformanceInfo(), new Font("Consolas", 6), Brushes.Red, 0, height);

        writableBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, width, height));
        writableBitmap.Unlock();

        writableBitmap.Freeze();

        return writableBitmap;
    }
}

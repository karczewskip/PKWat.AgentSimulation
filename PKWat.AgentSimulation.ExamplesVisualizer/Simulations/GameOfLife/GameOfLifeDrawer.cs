namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.GameOfLife;

using PKWat.AgentSimulation.Core;
using System.Drawing;
using System.Windows.Media.Imaging;

public class GameOfLifeDrawer : IVisualizationDrawer
{
    public BitmapSource Draw(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<LifeMatrixEnvironment>();

        var width = environment.GetWidth();
        var height = environment.GetHeight();
        var stride = width * 4;

        var writableBitmap = new WriteableBitmap(width, height, 96, 96, System.Windows.Media.PixelFormats.Bgr32, null);
        var pixels = new byte[width * height * 4];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var index = (i * height + j) * 4;
                if (environment.IsCellAlive(i, j))
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

        writableBitmap.Freeze();

        return writableBitmap;
    }

}

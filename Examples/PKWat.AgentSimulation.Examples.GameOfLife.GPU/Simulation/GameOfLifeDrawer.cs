namespace PKWat.AgentSimulation.Examples.GameOfLife.GPU.Simulation;

using PKWat.AgentSimulation.Core;
using System.Windows.Media.Imaging;

public class GameOfLifeDrawer
{
    public BitmapSource Draw(ISimulationContext<LifeMatrixEnvironment> context)
    {
        var width = context.SimulationEnvironment.GetWidth();
        var height = context.SimulationEnvironment.GetHeight();

        var bmp = new WriteableBitmap(width, height, 96, 96, System.Windows.Media.PixelFormats.Bgr32, null);
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
        bmp.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixels, width * 4, 0);
        return bmp;
    }
}

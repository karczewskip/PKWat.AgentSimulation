namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.LangtonAnts;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Examples.LangtonAnts;
using System.Windows.Media.Imaging;

public class LangtonAntsDrawer : IVisualizationDrawer
{
    private const int CellSize = 4; // Pixels per cell for better visibility

    public BitmapSource Draw(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<LangtonAntsEnvironment>();

        var width = environment.Width * CellSize;
        var height = environment.Height * CellSize;

        var writableBitmap = new WriteableBitmap(width, height, 96, 96, System.Windows.Media.PixelFormats.Bgr32, null);
        var pixels = new byte[width * height * 4];

        // Draw grid colors
        for (int x = 0; x < environment.Width; x++)
        {
            for (int y = 0; y < environment.Height; y++)
            {
                var color = environment.GetColorAt(x, y);
                
                // Fill the cell with the color
                for (int dx = 0; dx < CellSize; dx++)
                {
                    for (int dy = 0; dy < CellSize; dy++)
                    {
                        var pixelX = x * CellSize + dx;
                        var pixelY = y * CellSize + dy;
                        var index = (pixelY * width + pixelX) * 4;

                        pixels[index] = color.B;     // Blue
                        pixels[index + 1] = color.G; // Green
                        pixels[index + 2] = color.R; // Red
                        pixels[index + 3] = 255;     // Alpha
                    }
                }
            }
        }

        var ants = context.GetAgents<Ant>();
        // Draw ants as black dots in the center of their cells
        foreach (var ant in ants)
        {
            var centerX = ant.X * CellSize + CellSize / 2;
            var centerY = ant.Y * CellSize + CellSize / 2;

            // Draw ant as a small black square (2x2 pixels) in the center
            for (int dx = -1; dx <= 0; dx++)
            {
                for (int dy = -1; dy <= 0; dy++)
                {
                    var pixelX = centerX + dx;
                    var pixelY = centerY + dy;
                    
                    if (pixelX >= 0 && pixelX < width && pixelY >= 0 && pixelY < height)
                    {
                        var index = (pixelY * width + pixelX) * 4;
                        pixels[index] = 0;     // Black
                        pixels[index + 1] = 0;
                        pixels[index + 2] = 0;
                        pixels[index + 3] = 255;
                    }
                }
            }

            // Draw direction indicator (small line)
            var (dirX, dirY) = ant.Direction switch
            {
                AntDirection.North => (0, -1),
                AntDirection.East => (1, 0),
                AntDirection.South => (0, 1),
                AntDirection.West => (-1, 0),
                _ => (0, 0)
            };

            for (int i = 1; i <= 2; i++)
            {
                var pixelX = centerX + dirX * i;
                var pixelY = centerY + dirY * i;
                
                if (pixelX >= 0 && pixelX < width && pixelY >= 0 && pixelY < height)
                {
                    var index = (pixelY * width + pixelX) * 4;
                    pixels[index] = 255;     // White for direction
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

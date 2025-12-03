using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Genetics.PolynomialInterpolation;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.GeneticsPolynomialInterpolation;

public class GeneticsPolynomialAproximationDrawer : IVisualizationDrawer
{
    public BitmapSource Draw(ISimulationContext context)
    {
        var blackboard = context.GetSimulationEnvironment<CalculationsBlackboard>();
        var bestAgentId = blackboard.AgentErrors.OrderBy(kv => kv.Value.MeanAbsoluteError)
            .First()
            .Key;
        var bestAgent = context.GetRequiredAgent<PolynomialCheckAgent>(bestAgentId);
        var calculated = bestAgent.CalculateValues(blackboard.ExpectedValues.X);
        return CreateApproximationChart(
            blackboard.ExpectedValues.X,
            blackboard.ExpectedValues.Y,
            calculated);
    }

    private BitmapSource CreateApproximationChart(
        double[] xData,
        double[] yOriginal,
        double[] yApprox,
        int width = 800,
        int height = 600)
    {
        // 1. Initialize the Plot
        Plot myPlot = new();

        // 2. Add the Original Function (displayed as Scatter Points to show raw data)
        var originalPlot = myPlot.Add.Scatter(xData, yOriginal);
        originalPlot.LineWidth = 0; // No connecting lines
        originalPlot.MarkerSize = 5;
        originalPlot.Color = Colors.Blue;
        originalPlot.LegendText = "Original Data";

        // 3. Add the Approximated Function (displayed as a smooth Line)
        var approxPlot = myPlot.Add.Scatter(xData, yApprox);
        approxPlot.LineWidth = 2;
        approxPlot.MarkerSize = 0; // No markers, just line
        approxPlot.Color = Colors.Red;
        approxPlot.LegendText = "Approximation";

        // 4. Customize Chart Styling
        myPlot.Title("Function Approximation");
        myPlot.XLabel("Argument (x)");
        myPlot.YLabel("Value (y)");
        myPlot.ShowLegend();

        // Calculate range based on original data
        double minY = yOriginal.Min();
        double maxY = yOriginal.Max();

        // Calculate padding (e.g., 10% extra space top and bottom)
        double span = maxY - minY;

        // Handle edge case where all Y values are identical (span is 0)
        if (span == 0) span = 1.0;

        double padding = span * 0.1;

        // Set the strict limits. 
        // Any approximation value outside this range will be clipped visually.
        myPlot.Axes.SetLimitsY(minY - padding, maxY + padding);

        // 5. Render to Image and Convert to BitmapSource
        // ScottPlot 5 renders easily to byte arrays
        byte[] imageBytes = myPlot.GetImageBytes(width, height, ImageFormat.Png);

        return ConvertBytesToBitmapSource(imageBytes);
    }

    /// <summary>
    /// Helper method to convert a byte array (PNG/JPG) into a WPF BitmapSource.
    /// </summary>
    private BitmapSource ConvertBytesToBitmapSource(byte[] imageData)
    {
        using (var stream = new MemoryStream(imageData))
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // Load completely into memory
            bitmap.StreamSource = stream;
            bitmap.EndInit();

            // Freezing is crucial for performance and thread safety in WPF
            bitmap.Freeze();

            return bitmap;
        }
    }

}

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.SearchingProblems;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;
using PKWat.AgentSimulation.Examples.SearchingProblems;
using PKWat.AgentSimulation.Examples.SearchingProblems.Agents;
using System.Drawing;
using System.Windows.Media.Imaging;

public class SearchingDrawer : IVisualizationDrawer
{
    private const int Scale = 8;
    private Bitmap? _bmp;

    public void Initialize(int width, int height)
    {
        _bmp = new Bitmap(Scale * width, Scale * height);
        _bmp.SetResolution(96, 96);
    }

    public BitmapSource Draw(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<SearchingEnvironment>();

        if (_bmp == null)
        {
            Initialize((int)environment.SearchSpaceWidth, (int)environment.SearchSpaceHeight);
        }

        using var graphic = Graphics.FromImage(_bmp!);
        
        // Clear with dark background
        graphic.Clear(Color.FromArgb(20, 20, 30));

        // Draw all explored points as a heatmap
        DrawHeatmap(graphic, environment);

        // Draw all explored points
        DrawExploredPoints(graphic, environment);

        // Draw search agents
        DrawSearchAgents(graphic, context);

        // Draw best point found
        DrawBestPoint(graphic, environment);

        // Draw info text
        DrawInfoText(graphic, context, environment);

        var bitmapSource = _bmp.ConvertToBitmapSource();
        bitmapSource.Freeze();

        return bitmapSource;
    }

    private void DrawHeatmap(Graphics graphic, SearchingEnvironment environment)
    {
        // Create a grid-based heatmap showing the objective function landscape
        int gridSize = 5;
        int cellsX = (int)(environment.SearchSpaceWidth / gridSize);
        int cellsY = (int)(environment.SearchSpaceHeight / gridSize);

        for (int x = 0; x < cellsX; x++)
        {
            for (int y = 0; y < cellsY; y++)
            {
                double gridX = x * gridSize + gridSize / 2.0;
                double gridY = y * gridSize + gridSize / 2.0;
                
                // Calculate objective function value
                double value = CalculateObjectiveFunction(gridX, gridY);
                
                // Map value to color (blue=low, red=high)
                int colorValue = (int)System.Math.Clamp(value * 2.55, 0, 255);
                Color color = Color.FromArgb(50, colorValue, 0, 255 - colorValue);
                
                using var brush = new SolidBrush(color);
                graphic.FillRectangle(
                    brush,
                    Scale * x * gridSize,
                    Scale * y * gridSize,
                    Scale * gridSize,
                    Scale * gridSize);
            }
        }
    }

    private void DrawExploredPoints(Graphics graphic, SearchingEnvironment environment)
    {
        // Draw small dots for all explored points
        foreach (var point in environment.Points)
        {
            // Color based on value (blue=low, yellow=high)
            int intensity = (int)System.Math.Clamp(point.Value * 2.55, 0, 255);
            Color color = Color.FromArgb(120, intensity, intensity, 255 - intensity);
            
            using var brush = new SolidBrush(color);
            graphic.FillEllipse(
                brush,
                Scale * (float)point.X - 1,
                Scale * (float)point.Y - 1,
                2,
                2);
        }
    }

    private void DrawSearchAgents(Graphics graphic, ISimulationContext context)
    {
        var agents = context.GetAgents<SearchAgent>().ToArray();
        
        for (int i = 0; i < agents.Length; i++)
        {
            var agent = agents[i];
            
            // Different color for each agent
            Color agentColor = i switch
            {
                0 => Color.FromArgb(200, 255, 100, 100), // Red
                1 => Color.FromArgb(200, 100, 255, 100), // Green
                2 => Color.FromArgb(200, 100, 100, 255), // Blue
                3 => Color.FromArgb(200, 255, 255, 100), // Yellow
                4 => Color.FromArgb(200, 255, 100, 255), // Magenta
                _ => Color.FromArgb(200, 150, 150, 150)  // Gray
            };

            using var brush = new SolidBrush(agentColor);
            using var pen = new Pen(Color.White, 1);
            
            // Draw agent as a circle with white outline
            float agentSize = 6;
            graphic.FillEllipse(
                brush,
                Scale * (float)agent.CurrentX - agentSize / 2,
                Scale * (float)agent.CurrentY - agentSize / 2,
                agentSize,
                agentSize);
            
            graphic.DrawEllipse(
                pen,
                Scale * (float)agent.CurrentX - agentSize / 2,
                Scale * (float)agent.CurrentY - agentSize / 2,
                agentSize,
                agentSize);
        }
    }

    private void DrawBestPoint(Graphics graphic, SearchingEnvironment environment)
    {
        if (environment.BestPoint == null)
            return;

        // Draw best point as a large glowing star
        using var brush = new SolidBrush(Color.FromArgb(220, 255, 215, 0));
        using var pen = new Pen(Color.White, 2);
        
        float starSize = 10;
        float x = Scale * (float)environment.BestPoint.X;
        float y = Scale * (float)environment.BestPoint.Y;

        // Draw a star shape
        PointF[] starPoints = new PointF[10];
        for (int i = 0; i < 10; i++)
        {
            double angle = i * System.Math.PI / 5 - System.Math.PI / 2;
            float radius = (i % 2 == 0) ? starSize : starSize / 2.5f;
            starPoints[i] = new PointF(
                x + (float)(radius * System.Math.Cos(angle)),
                y + (float)(radius * System.Math.Sin(angle)));
        }

        graphic.FillPolygon(brush, starPoints);
        graphic.DrawPolygon(pen, starPoints);
    }

    private void DrawInfoText(Graphics graphic, ISimulationContext context, SearchingEnvironment environment)
    {
        using var font = new Font("Arial", 10, FontStyle.Bold);
        using var brush = new SolidBrush(Color.White);
        using var shadowBrush = new SolidBrush(Color.Black);

        string info = $"Iteration: {context.Time.StepNo}\n" +
                      $"Points Explored: {environment.Points.Count}\n" +
                      $"Best Value: {environment.BestPoint?.Value:F2}\n" +
                      $"Best Position: ({environment.BestPoint?.X:F1}, {environment.BestPoint?.Y:F1})";

        // Draw shadow
        graphic.DrawString(info, font, shadowBrush, 11, 11);
        // Draw text
        graphic.DrawString(info, font, brush, 10, 10);
    }

    private double CalculateObjectiveFunction(double x, double y)
    {
        // Same objective function as in the simulation
        double centerX = 50.0;
        double centerY = 50.0;
        return 100.0 - System.Math.Pow(x - centerX, 2) / 25.0 - System.Math.Pow(y - centerY, 2) / 25.0;
    }
}

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.DifferentialEquation;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;
using PKWat.AgentSimulation.Examples.DifferentialEquation;
using PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;
using System.Drawing;
using System.Windows.Media.Imaging;

public class DifferentialEquationDrawer : IVisualizationDrawer
{
    private const int Width = 800;
    private const int Height = 600;
    private const int Padding = 50;
    private Bitmap? _bmp;

    public void Initialize()
    {
        _bmp = new Bitmap(Width, Height);
        _bmp.SetResolution(96, 96);
    }

    public BitmapSource Draw(ISimulationContext context)
    {
        if (_bmp == null)
        {
            Initialize();
        }

        var environment = context.GetSimulationEnvironment<DifferentialEquationEnvironment>();
        
        using var graphic = Graphics.FromImage(_bmp!);
        graphic.Clear(Color.White);
        graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var analyticalAgent = context.GetAgents<AnalyticalSolverAgent>().FirstOrDefault();
        var eulerAgent = context.GetAgents<EulerMethodAgent>().FirstOrDefault();
        var rkAgent = context.GetAgents<RungeKuttaMethodAgent>().FirstOrDefault();

        // Calculate Y range for all agents
        var allPoints = new List<(double X, double Y)>();
        if (analyticalAgent != null) allPoints.AddRange(analyticalAgent.SolutionPoints);
        if (eulerAgent != null) allPoints.AddRange(eulerAgent.SolutionPoints);
        if (rkAgent != null) allPoints.AddRange(rkAgent.SolutionPoints);

        double maxY = allPoints.Any() ? allPoints.Max(p => p.Y) : 1;
        double minY = allPoints.Any() ? System.Math.Min(0, allPoints.Min(p => p.Y)) : 0;

        DrawAxes(graphic, environment);
        DrawYAxisLabels(graphic, minY, maxY);
        DrawSolutions(graphic, context, environment, minY, maxY);
        DrawLegend(graphic, context);

        var bitmapSource = _bmp.ConvertToBitmapSource();
        bitmapSource.Freeze();

        return bitmapSource;
    }

    private void DrawAxes(Graphics graphic, DifferentialEquationEnvironment environment)
    {
        using var pen = new Pen(Color.Black, 2);
        using var font = new Font("Arial", 10);

        graphic.DrawLine(pen, Padding, Height - Padding, Width - Padding, Height - Padding);
        graphic.DrawLine(pen, Padding, Padding, Padding, Height - Padding);

        graphic.DrawString("x", font, Brushes.Black, Width - Padding + 10, Height - Padding - 10);
        graphic.DrawString("y", font, Brushes.Black, Padding - 20, Padding - 10);

        // X-axis labels
        for (int i = 0; i <= 10; i++)
        {
            double x = environment.StartX + i * (environment.EndX - environment.StartX) / 10;
            int screenX = Padding + (int)((Width - 2 * Padding) * i / 10.0);
            graphic.DrawString(x.ToString("F1"), font, Brushes.Black, screenX - 10, Height - Padding + 5);
        }
    }

    private void DrawYAxisLabels(Graphics graphic, double minY, double maxY)
    {
        using var font = new Font("Arial", 9);
        
        for (int i = 0; i <= 10; i++)
        {
            double y = minY + i * (maxY - minY) / 10;
            int screenY = Height - Padding - (int)((Height - 2 * Padding) * i / 10.0);
            
            string label = y.ToString("F1");
            var labelSize = graphic.MeasureString(label, font);
            graphic.DrawString(label, font, Brushes.Black, Padding - labelSize.Width - 5, screenY - labelSize.Height / 2);
            
            using var tickPen = new Pen(Color.Black, 1);
            graphic.DrawLine(tickPen, Padding - 3, screenY, Padding, screenY);
        }
    }

    private void DrawSolutions(Graphics graphic, ISimulationContext context, DifferentialEquationEnvironment environment, double minY, double maxY)
    {
        var analyticalAgent = context.GetAgents<AnalyticalSolverAgent>().FirstOrDefault();
        var eulerAgent = context.GetAgents<EulerMethodAgent>().FirstOrDefault();
        var rkAgent = context.GetAgents<RungeKuttaMethodAgent>().FirstOrDefault();

        if (analyticalAgent != null && analyticalAgent.SolutionPoints.Count > 1)
        {
            DrawCurve(graphic, analyticalAgent.SolutionPoints, environment, Color.Blue, 3, minY, maxY);
        }

        if (eulerAgent != null && eulerAgent.SolutionPoints.Count > 1)
        {
            DrawCurve(graphic, eulerAgent.SolutionPoints, environment, Color.Red, 2, minY, maxY);
        }

        if (rkAgent != null && rkAgent.SolutionPoints.Count > 1)
        {
            DrawCurve(graphic, rkAgent.SolutionPoints, environment, Color.Green, 2, minY, maxY);
        }
    }

    private void DrawCurve(Graphics graphic, List<(double X, double Y)> points, DifferentialEquationEnvironment environment, Color color, int width, double minY, double maxY)
    {
        if (points.Count < 2)
            return;

        double rangeY = maxY - minY;
        if (rangeY == 0) rangeY = 1;

        using var pen = new Pen(color, width);

        for (int i = 0; i < points.Count - 1; i++)
        {
            var p1 = points[i];
            var p2 = points[i + 1];

            int x1 = Padding + (int)((Width - 2 * Padding) * (p1.X - environment.StartX) / (environment.EndX - environment.StartX));
            int y1 = Height - Padding - (int)((Height - 2 * Padding) * (p1.Y - minY) / rangeY);
            int x2 = Padding + (int)((Width - 2 * Padding) * (p2.X - environment.StartX) / (environment.EndX - environment.StartX));
            int y2 = Height - Padding - (int)((Height - 2 * Padding) * (p2.Y - minY) / rangeY);

            graphic.DrawLine(pen, x1, y1, x2, y2);
        }
    }

    private void DrawLegend(Graphics graphic, ISimulationContext context)
    {
        using var font = new Font("Arial", 10, FontStyle.Bold);
        int legendX = Width - 180;
        int legendY = 20;
        int lineHeight = 25;

        var analyticalAgent = context.GetAgents<AnalyticalSolverAgent>().FirstOrDefault();
        var eulerAgent = context.GetAgents<EulerMethodAgent>().FirstOrDefault();
        var rkAgent = context.GetAgents<RungeKuttaMethodAgent>().FirstOrDefault();

        graphic.DrawString($"Iteration: {context.Time.StepNo}", font, Brushes.Black, legendX, legendY);
        legendY += lineHeight;

        if (analyticalAgent != null)
        {
            using var pen = new Pen(Color.Blue, 3);
            graphic.DrawLine(pen, legendX, legendY + 7, legendX + 30, legendY + 7);
            graphic.DrawString("Analytical", font, Brushes.Black, legendX + 40, legendY);
            graphic.DrawString($"y = {analyticalAgent.CurrentY:F4}", new Font("Arial", 9), Brushes.Black, legendX + 40, legendY + 15);
            legendY += lineHeight * 2;
        }

        if (eulerAgent != null)
        {
            using var pen = new Pen(Color.Red, 2);
            graphic.DrawLine(pen, legendX, legendY + 7, legendX + 30, legendY + 7);
            graphic.DrawString("Euler", font, Brushes.Black, legendX + 40, legendY);
            graphic.DrawString($"y = {eulerAgent.CurrentY:F4}", new Font("Arial", 9), Brushes.Black, legendX + 40, legendY + 15);
            if (analyticalAgent != null)
            {
                double error = System.Math.Abs(eulerAgent.CurrentY - analyticalAgent.CurrentY);
                graphic.DrawString($"Error: {error:E2}", new Font("Arial", 8), Brushes.DarkRed, legendX + 40, legendY + 30);
                legendY += lineHeight * 2 + 15;
            }
            else
            {
                legendY += lineHeight * 2;
            }
        }

        if (rkAgent != null)
        {
            using var pen = new Pen(Color.Green, 2);
            graphic.DrawLine(pen, legendX, legendY + 7, legendX + 30, legendY + 7);
            graphic.DrawString("Runge-Kutta", font, Brushes.Black, legendX + 40, legendY);
            graphic.DrawString($"y = {rkAgent.CurrentY:F4}", new Font("Arial", 9), Brushes.Black, legendX + 40, legendY + 15);
            if (analyticalAgent != null)
            {
                double error = System.Math.Abs(rkAgent.CurrentY - analyticalAgent.CurrentY);
                graphic.DrawString($"Error: {error:E2}", new Font("Arial", 8), Brushes.DarkGreen, legendX + 40, legendY + 30);
            }
        }
    }
}

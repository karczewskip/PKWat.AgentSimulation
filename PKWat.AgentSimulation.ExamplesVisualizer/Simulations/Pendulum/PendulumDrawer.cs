namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Pendulum;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;
using PKWat.AgentSimulation.Examples.Pendulum;
using PKWat.AgentSimulation.Examples.Pendulum.Agents;
using System.Drawing;
using System.Windows.Media.Imaging;

public class PendulumDrawer : IVisualizationDrawer
{
    private const int Width = 1200;
    private const int Height = 600;
    private const int GraphPadding = 50;
    private const int PendulumSize = 350;
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

        var environment = context.GetSimulationEnvironment<PendulumEnvironment>();
        
        using var graphic = Graphics.FromImage(_bmp!);
        graphic.Clear(Color.White);
        graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var agents = context.GetAgents<PendulumSolverAgent>().ToList();

        // Draw pendulum visualizations on the left
        DrawPendulumVisualizations(graphic, agents, environment);

        // Draw angle vs time graph on the right
        DrawAngleGraph(graphic, agents, environment);

        var bitmapSource = _bmp.ConvertToBitmapSource();
        bitmapSource.Freeze();

        return bitmapSource;
    }

    private void DrawPendulumVisualizations(Graphics graphic, List<PendulumSolverAgent> agents, PendulumEnvironment environment)
    {
        int centerX = PendulumSize / 2;
        int spacing = 120;
        int startY = 100;

        var colors = new Dictionary<string, Color>
        {
            { "Analytical", Color.Blue },
            { "Euler", Color.Red },
            { "RK4", Color.Green }
        };

        using var font = new Font("Arial", 10, FontStyle.Bold);
        using var titleFont = new Font("Arial", 12, FontStyle.Bold);

        graphic.DrawString("Pendulum Visualizations", titleFont, Brushes.Black, 10, 10);

        for (int i = 0; i < agents.Count; i++)
        {
            var agent = agents[i];
            int yPos = startY + i * spacing;

            Color color = colors.ContainsKey(agent.SolverName) ? colors[agent.SolverName] : Color.Black;

            // Draw title
            graphic.DrawString(agent.SolverName, font, new SolidBrush(color), centerX - 40, yPos - 30);

            // Draw pivot point
            graphic.FillEllipse(Brushes.Black, centerX - 5, yPos - 5, 10, 10);

            // Calculate pendulum bob position
            double theta = agent.CurrentState.Theta;
            int length = 80;
            int bobX = centerX + (int)(length * Math.Sin(theta));
            int bobY = yPos + (int)(length * Math.Cos(theta));

            // Draw rod
            using var pen = new Pen(color, 3);
            graphic.DrawLine(pen, centerX, yPos, bobX, bobY);

            // Draw bob
            graphic.FillEllipse(new SolidBrush(color), bobX - 8, bobY - 8, 16, 16);

            // Draw info
            using var infoFont = new Font("Arial", 9);
            graphic.DrawString($"Theta = {agent.CurrentState.Theta:F3} rad ({agent.CurrentState.Theta * 180 / Math.PI:F1} deg)", 
                infoFont, Brushes.Black, centerX + 100, yPos - 20);
            graphic.DrawString($"Omega = {agent.CurrentState.Omega:F3} rad/s", 
                infoFont, Brushes.Black, centerX + 100, yPos);
            graphic.DrawString($"t = {agent.CurrentTime:F2} s", 
                infoFont, Brushes.Black, centerX + 100, yPos + 20);
        }
    }

    private void DrawAngleGraph(Graphics graphic, List<PendulumSolverAgent> agents, PendulumEnvironment environment)
    {
        int graphLeft = PendulumSize + 100;
        int graphWidth = Width - graphLeft - GraphPadding;
        int graphHeight = Height - 2 * GraphPadding;
        int graphTop = GraphPadding;
        int graphBottom = Height - GraphPadding;

        using var axisPen = new Pen(Color.Black, 2);
        using var font = new Font("Arial", 10);
        using var titleFont = new Font("Arial", 12, FontStyle.Bold);

        // Title
        graphic.DrawString("Angle vs Time", titleFont, Brushes.Black, graphLeft, 10);

        // Draw axes
        graphic.DrawLine(axisPen, graphLeft, graphBottom, graphLeft + graphWidth, graphBottom);
        graphic.DrawLine(axisPen, graphLeft, graphTop, graphLeft, graphBottom);

        // Labels
        graphic.DrawString("Time (s)", font, Brushes.Black, graphLeft + graphWidth / 2 - 30, graphBottom + 25);
        
        using (var transform = graphic.Transform.Clone())
        {
            graphic.TranslateTransform(graphLeft - 35, graphTop + graphHeight / 2);
            graphic.RotateTransform(-90);
            graphic.DrawString("Angle (rad)", font, Brushes.Black, 0, 0);
            graphic.Transform = transform;
        }

        // Find data range
        var allPoints = agents.SelectMany(a => a.StateHistory).ToList();
        if (!allPoints.Any())
            return;

        double maxTime = environment.TotalTime;
        double maxTheta = allPoints.Max(p => Math.Abs(p.Theta));
        double minTheta = -maxTheta;
        double rangeTheta = maxTheta - minTheta;
        if (rangeTheta == 0) rangeTheta = 1;

        // Draw grid and labels
        DrawGraphGrid(graphic, graphLeft, graphWidth, graphTop, graphHeight, maxTime, minTheta, maxTheta);

        // Draw curves
        var colors = new Dictionary<string, (Color color, int width)>
        {
            { "Analytical", (Color.Blue, 3) },
            { "Euler", (Color.Red, 2) },
            { "RK4", (Color.Green, 2) }
        };

        foreach (var agent in agents)
        {
            if (agent.StateHistory.Count < 2)
                continue;

            var (color, width) = colors.ContainsKey(agent.SolverName) 
                ? colors[agent.SolverName] 
                : (Color.Black, 2);

            DrawGraphCurve(graphic, agent.StateHistory, graphLeft, graphWidth, graphTop, graphHeight, 
                maxTime, minTheta, rangeTheta, color, width);
        }

        // Draw legend
        DrawGraphLegend(graphic, agents, graphLeft + graphWidth - 150, graphTop + 10);
    }

    private void DrawGraphGrid(Graphics graphic, int left, int width, int top, int height, 
        double maxTime, double minTheta, double maxTheta)
    {
        using var gridPen = new Pen(Color.LightGray, 1);
        using var font = new Font("Arial", 8);

        // Vertical grid lines (time)
        for (int i = 0; i <= 10; i++)
        {
            double time = i * maxTime / 10;
            int x = left + (int)(width * i / 10.0);
            
            graphic.DrawLine(gridPen, x, top, x, top + height);
            graphic.DrawString(time.ToString("F1"), font, Brushes.Black, x - 10, top + height + 5);
        }

        // Horizontal grid lines (theta)
        for (int i = 0; i <= 10; i++)
        {
            double theta = minTheta + i * (maxTheta - minTheta) / 10;
            int y = top + height - (int)(height * i / 10.0);
            
            graphic.DrawLine(gridPen, left, y, left + width, y);
            
            string label = theta.ToString("F2");
            var labelSize = graphic.MeasureString(label, font);
            graphic.DrawString(label, font, Brushes.Black, left - labelSize.Width - 5, y - labelSize.Height / 2);
        }
    }

    private void DrawGraphCurve(Graphics graphic, List<(double Time, double Theta, double Omega)> points,
        int left, int width, int top, int height, double maxTime, double minTheta, double rangeTheta,
        Color color, int penWidth)
    {
        if (points.Count < 2)
            return;

        using var pen = new Pen(color, penWidth);

        for (int i = 0; i < points.Count - 1; i++)
        {
            var p1 = points[i];
            var p2 = points[i + 1];

            int x1 = left + (int)(width * p1.Time / maxTime);
            int y1 = top + height - (int)(height * (p1.Theta - minTheta) / rangeTheta);
            int x2 = left + (int)(width * p2.Time / maxTime);
            int y2 = top + height - (int)(height * (p2.Theta - minTheta) / rangeTheta);

            graphic.DrawLine(pen, x1, y1, x2, y2);
        }
    }

    private void DrawGraphLegend(Graphics graphic, List<PendulumSolverAgent> agents, int x, int y)
    {
        using var font = new Font("Arial", 10, FontStyle.Bold);
        var colors = new Dictionary<string, Color>
        {
            { "Analytical", Color.Blue },
            { "Euler", Color.Red },
            { "RK4", Color.Green }
        };

        int lineHeight = 25;

        foreach (var agent in agents)
        {
            Color color = colors.ContainsKey(agent.SolverName) ? colors[agent.SolverName] : Color.Black;
            
            using var pen = new Pen(color, 3);
            graphic.DrawLine(pen, x, y + 7, x + 30, y + 7);
            graphic.DrawString(agent.SolverName, font, new SolidBrush(color), x + 40, y);
            
            y += lineHeight;
        }
    }
}

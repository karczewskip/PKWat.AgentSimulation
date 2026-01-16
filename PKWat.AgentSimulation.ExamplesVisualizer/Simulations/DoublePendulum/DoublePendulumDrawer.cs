namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.DoublePendulum;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;
using PKWat.AgentSimulation.Examples.DoublePendulum;
using PKWat.AgentSimulation.Examples.DoublePendulum.Agents;
using PKWat.AgentSimulation.SimMath.Algorithms.DoublePendulum;
using System.Drawing;
using System.Windows.Media.Imaging;

public class DoublePendulumDrawer : IVisualizationDrawer
{
    private const int Width = 1600;
    private const int Height = 600;
    private const int Padding = 50;
    private Bitmap? _bmp;

    private static readonly Color[] ColorPalette =
    [
        Color.Blue,
        Color.Red,
        Color.Green,
        Color.Orange,
        Color.Purple,
        Color.Brown,
        Color.Magenta,
        Color.Teal
    ];

    public void Initialize()
    {
        _bmp = new Bitmap(Width, Height);
        _bmp.SetResolution(96, 96);
    }

    private Color GetAgentColor(int agentIndex)
    {
        return ColorPalette[agentIndex % ColorPalette.Length];
    }

    public BitmapSource Draw(ISimulationContext context)
    {
        if (_bmp == null)
        {
            Initialize();
        }

        var environment = context.GetSimulationEnvironment<DoublePendulumEnvironment>();
        
        using var graphic = Graphics.FromImage(_bmp!);
        graphic.Clear(Color.White);
        graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var agents = context.GetAgents<DoublePendulumSolverAgent>().ToList();

        // Draw three visualizations side by side
        DrawPhysicalModel(graphic, agents, environment, 0);
        DrawTrajectory(graphic, agents, environment, 500);
        DrawAngleGraph(graphic, agents, environment, 1050);

        var bitmapSource = _bmp.ConvertToBitmapSource();
        bitmapSource.Freeze();

        return bitmapSource;
    }

    private void DrawPhysicalModel(Graphics graphic, List<DoublePendulumSolverAgent> agents, 
        DoublePendulumEnvironment environment, int offsetX)
    {
        using var font = new Font("Arial", 10, FontStyle.Bold);
        using var titleFont = new Font("Arial", 12, FontStyle.Bold);
        using var infoFont = new Font("Arial", 9);

        int centerX = offsetX + 250;
        int centerY = 150;
        int scale = 100;

        graphic.DrawString("Physical Model", titleFont, Brushes.Black, offsetX + 10, 10);

        for (int i = 0; i < agents.Count; i++)
        {
            var agent = agents[i];
            Color color = GetAgentColor(i);

            if (agent.StateHistory.Count == 0)
                continue;

            var lastState = agent.StateHistory[^1];

            // Calculate positions
            int x1 = centerX + (int)(lastState.X1 * scale);
            int y1 = centerY + (int)(-lastState.Y1 * scale);
            int x2 = centerX + (int)(lastState.X2 * scale);
            int y2 = centerY + (int)(-lastState.Y2 * scale);

            // Draw pivot point
            graphic.FillEllipse(Brushes.Black, centerX - 5, centerY - 5, 10, 10);

            // Draw first rod
            using var pen = new Pen(color, 3);
            graphic.DrawLine(pen, centerX, centerY, x1, y1);

            // Draw first bob
            graphic.FillEllipse(new SolidBrush(color), x1 - 10, y1 - 10, 20, 20);

            // Draw second rod
            graphic.DrawLine(pen, x1, y1, x2, y2);

            // Draw second bob
            graphic.FillEllipse(new SolidBrush(color), x2 - 12, y2 - 12, 24, 24);

            // Draw info
            int infoY = 400 + i * 60;
            graphic.DrawString($"{agent.SolverName}", font, new SolidBrush(color), offsetX + 10, infoY);
            graphic.DrawString($"theta1 = {agent.CurrentState.Theta1:F3} rad ({agent.CurrentState.Theta1 * 180 / Math.PI:F1} deg)", 
                infoFont, Brushes.Black, offsetX + 10, infoY + 20);
            graphic.DrawString($"theta2 = {agent.CurrentState.Theta2:F3} rad ({agent.CurrentState.Theta2 * 180 / Math.PI:F1} deg)", 
                infoFont, Brushes.Black, offsetX + 10, infoY + 35);
            graphic.DrawString($"t = {agent.CurrentTime:F2} s", 
                infoFont, Brushes.Black, offsetX + 250, infoY + 20);
        }
    }

    private void DrawTrajectory(Graphics graphic, List<DoublePendulumSolverAgent> agents, 
        DoublePendulumEnvironment environment, int offsetX)
    {
        using var titleFont = new Font("Arial", 12, FontStyle.Bold);
        
        int centerX = offsetX + 250;
        int centerY = 300;
        int scale = 100;

        graphic.DrawString("Trajectory Path", titleFont, Brushes.Black, offsetX + 10, 10);

        // Draw axes
        using var axisPen = new Pen(Color.LightGray, 1);
        graphic.DrawLine(axisPen, centerX - 200, centerY, centerX + 200, centerY);
        graphic.DrawLine(axisPen, centerX, centerY - 200, centerX, centerY + 200);

        // Draw pivot point
        graphic.FillEllipse(Brushes.Black, centerX - 5, centerY - 5, 10, 10);

        for (int i = 0; i < agents.Count; i++)
        {
            var agent = agents[i];
            Color color = GetAgentColor(i);

            if (agent.StateHistory.Count < 2)
                continue;

            // Draw trajectory of second pendulum bob
            using var pen = new Pen(Color.FromArgb(128, color), 2);
            
            for (int j = 1; j < agent.StateHistory.Count; j++)
            {
                var prev = agent.StateHistory[j - 1];
                var curr = agent.StateHistory[j];

                int x1 = centerX + (int)(prev.X2 * scale);
                int y1 = centerY + (int)(-prev.Y2 * scale);
                int x2 = centerX + (int)(curr.X2 * scale);
                int y2 = centerY + (int)(-curr.Y2 * scale);

                graphic.DrawLine(pen, x1, y1, x2, y2);
            }

            // Highlight current position
            if (agent.StateHistory.Count > 0)
            {
                var last = agent.StateHistory[^1];
                int x = centerX + (int)(last.X2 * scale);
                int y = centerY + (int)(-last.Y2 * scale);
                graphic.FillEllipse(new SolidBrush(color), x - 5, y - 5, 10, 10);
            }
        }
    }

    private void DrawAngleGraph(Graphics graphic, List<DoublePendulumSolverAgent> agents, 
        DoublePendulumEnvironment environment, int offsetX)
    {
        int graphWidth = 500;
        int graphHeight = 500;
        int graphLeft = offsetX + Padding;
        int graphTop = Padding;
        int graphBottom = graphTop + graphHeight;

        using var axisPen = new Pen(Color.Black, 2);
        using var font = new Font("Arial", 10);
        using var titleFont = new Font("Arial", 12, FontStyle.Bold);

        // Title
        graphic.DrawString("Angles vs Time", titleFont, Brushes.Black, offsetX + 10, 10);

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
        if (agents.Count == 0 || agents[0].StateHistory.Count == 0)
            return;

        double maxTime = environment.TotalTime;
        double maxTheta = Math.PI;
        double minTheta = -Math.PI;
        double rangeTheta = maxTheta - minTheta;

        // Draw grid
        DrawGraphGrid(graphic, graphLeft, graphWidth, graphTop, graphHeight, maxTime, minTheta, maxTheta);

        // Draw curves for each agent
        for (int i = 0; i < agents.Count; i++)
        {
            var agent = agents[i];
            if (agent.StateHistory.Count < 2)
                continue;

            Color color = GetAgentColor(i);

            // Draw theta1
            DrawAngleCurve(graphic, agent.StateHistory, graphLeft, graphWidth, graphTop, graphHeight, 
                maxTime, minTheta, rangeTheta, color, 2, true);

            // Draw theta2 with dashed line
            DrawAngleCurve(graphic, agent.StateHistory, graphLeft, graphWidth, graphTop, graphHeight, 
                maxTime, minTheta, rangeTheta, Color.FromArgb(180, color), 2, false);
        }

        // Draw legend
        DrawAngleLegend(graphic, agents, graphLeft + graphWidth - 150, graphTop + 10);
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
            graphic.DrawString(theta.ToString("F1"), font, Brushes.Black, left - 40, y - 7);
        }
    }

    private void DrawAngleCurve(Graphics graphic, 
        List<(double Time, DoublePendulumState State, double X1, double Y1, double X2, double Y2)> history,
        int left, int width, int top, int height, 
        double maxTime, double minTheta, double rangeTheta, Color color, int penWidth, bool isTheta1)
    {
        using var pen = new Pen(color, penWidth);
        if (!isTheta1)
        {
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
        }

        for (int j = 1; j < history.Count; j++)
        {
            var prev = history[j - 1];
            var curr = history[j];

            double prevTheta = isTheta1 ? prev.State.Theta1 : prev.State.Theta2;
            double currTheta = isTheta1 ? curr.State.Theta1 : curr.State.Theta2;

            int x1 = left + (int)(width * prev.Time / maxTime);
            int y1 = top + height - (int)(height * (prevTheta - minTheta) / rangeTheta);
            int x2 = left + (int)(width * curr.Time / maxTime);
            int y2 = top + height - (int)(height * (currTheta - minTheta) / rangeTheta);

            // Clamp to visible area
            if (y1 >= top && y1 <= top + height && y2 >= top && y2 <= top + height)
            {
                graphic.DrawLine(pen, x1, y1, x2, y2);
            }
        }
    }

    private void DrawAngleLegend(Graphics graphic, List<DoublePendulumSolverAgent> agents, int x, int y)
    {
        using var font = new Font("Arial", 9);
        using var titleFont = new Font("Arial", 10, FontStyle.Bold);

        graphic.DrawString("Legend:", titleFont, Brushes.Black, x, y);
        y += 20;

        for (int i = 0; i < agents.Count; i++)
        {
            var agent = agents[i];
            Color color = GetAgentColor(i);

            // Solid line for theta1
            using var solidPen = new Pen(color, 2);
            graphic.DrawLine(solidPen, x, y + 5, x + 30, y + 5);
            graphic.DrawString($"theta1 - {agent.SolverName}", font, Brushes.Black, x + 35, y);

            y += 20;

            // Dashed line for theta2
            using var dashedPen = new Pen(Color.FromArgb(180, color), 2);
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            graphic.DrawLine(dashedPen, x, y + 5, x + 30, y + 5);
            graphic.DrawString($"theta2 - {agent.SolverName}", font, Brushes.Black, x + 35, y);

            y += 25;
        }
    }
}

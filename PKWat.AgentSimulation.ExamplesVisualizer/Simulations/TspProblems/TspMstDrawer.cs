namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.TspProblems;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;
using PKWat.AgentSimulation.Examples.TspProblems;
using PKWat.AgentSimulation.Examples.TspProblems.Agents;
using System.Drawing;
using System.Windows.Media.Imaging;

public class TspMstDrawer : IVisualizationDrawer
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
        var environment = context.GetSimulationEnvironment<TspEnvironment>();

        if (_bmp == null)
        {
            Initialize((int)environment.SearchSpaceWidth, (int)environment.SearchSpaceHeight);
        }

        using var graphic = Graphics.FromImage(_bmp!);
        
        graphic.Clear(Color.FromArgb(20, 20, 30));

        var agent = context.GetAgents<MstAgent>().FirstOrDefault();

        // Draw all TSP points
        DrawTspPoints(graphic, environment, agent);

        // Draw MST route (full path after DFS)
        if (agent?.MstRoute != null && agent.MstRoute.Count > 0)
        {
            DrawRoute(graphic, environment, agent.MstRoute, Color.FromArgb(100, 150, 150, 255), 2);
        }

        // Draw current partial route being built
        if (agent?.CurrentRoute != null && agent.CurrentRoute.Count > 1)
        {
            DrawRoute(graphic, environment, agent.CurrentRoute, Color.FromArgb(200, 255, 255, 0), 3);
        }

        // Draw best solution found
        if (agent?.BestSolution != null && agent.BestSolution.Route.Count > 0)
        {
            DrawRoute(graphic, environment, agent.BestSolution.Route, Color.FromArgb(200, 0, 255, 0), 3);
        }

        // Draw point labels
        DrawPointLabels(graphic, environment);

        // Draw info text
        DrawInfoText(graphic, context, environment, agent);

        var bitmapSource = _bmp.ConvertToBitmapSource();
        bitmapSource.Freeze();

        return bitmapSource;
    }

    private void DrawTspPoints(Graphics graphic, TspEnvironment environment, MstAgent? agent)
    {
        foreach (var point in environment.Points)
        {
            // Different color for visited nodes
            bool isVisited = agent?.VisitedNodes?.Contains(point.Id) ?? false;
            Color pointColor = isVisited 
                ? Color.FromArgb(255, 100, 255, 100) 
                : Color.FromArgb(255, 255, 200, 100);

            using var brush = new SolidBrush(pointColor);
            using var pen = new Pen(Color.White, 2);
            
            float size = 12;
            graphic.FillEllipse(
                brush,
                Scale * (float)point.X - size / 2,
                Scale * (float)point.Y - size / 2,
                size,
                size);
            
            graphic.DrawEllipse(
                pen,
                Scale * (float)point.X - size / 2,
                Scale * (float)point.Y - size / 2,
                size,
                size);
        }
    }

    private void DrawRoute(Graphics graphic, TspEnvironment environment, List<int> route, Color color, int width)
    {
        if (route.Count < 2)
            return;

        using var pen = new Pen(color, width);

        for (int i = 0; i < route.Count - 1; i++)
        {
            if (route[i] >= environment.Points.Count || route[i + 1] >= environment.Points.Count)
                continue;

            var p1 = environment.Points[route[i]];
            var p2 = environment.Points[route[i + 1]];

            graphic.DrawLine(
                pen,
                Scale * (float)p1.X,
                Scale * (float)p1.Y,
                Scale * (float)p2.X,
                Scale * (float)p2.Y);
        }

        // Draw return to start only for complete routes
        if (route.Count > 2)
        {
            var last = environment.Points[route[^1]];
            var first = environment.Points[route[0]];
            using var dashedPen = new Pen(color, width);
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            
            graphic.DrawLine(
                dashedPen,
                Scale * (float)last.X,
                Scale * (float)last.Y,
                Scale * (float)first.X,
                Scale * (float)first.Y);
        }
    }

    private void DrawPointLabels(Graphics graphic, TspEnvironment environment)
    {
        using var font = new Font("Arial", 8, FontStyle.Bold);
        using var brush = new SolidBrush(Color.White);
        using var shadowBrush = new SolidBrush(Color.Black);

        foreach (var point in environment.Points)
        {
            string label = point.Id.ToString();
            var size = graphic.MeasureString(label, font);

            float x = Scale * (float)point.X - size.Width / 2;
            float y = Scale * (float)point.Y - size.Height / 2;

            graphic.DrawString(label, font, shadowBrush, x + 1, y + 1);
            graphic.DrawString(label, font, brush, x, y);
        }
    }

    private void DrawInfoText(Graphics graphic, ISimulationContext context, TspEnvironment environment, MstAgent? agent)
    {
        using var font = new Font("Arial", 10, FontStyle.Bold);
        using var brush = new SolidBrush(Color.White);
        using var shadowBrush = new SolidBrush(Color.Black);

        string mstStatus = agent?.IsMstBuilt == true ? "Built" : "Not Built";
        int nodesAdded = agent?.CurrentRoute?.Count ?? 0;
        
        string info = $"Algorithm: MST Approximation (Prim's)\n" +
                      $"Iteration: {context.Time.StepNo}\n" +
                      $"Points: {environment.Points.Count}\n" +
                      $"MST: {mstStatus}\n" +
                      $"Nodes Added: {nodesAdded}/{environment.Points.Count}\n" +
                      $"Best Distance: {agent?.BestSolution?.TotalDistance:F2}\n" +
                      $"Status: {(agent?.IsComplete == true ? "COMPLETE" : "Running")}";

        graphic.DrawString(info, font, shadowBrush, 11, 11);
        graphic.DrawString(info, font, brush, 10, 10);
    }
}

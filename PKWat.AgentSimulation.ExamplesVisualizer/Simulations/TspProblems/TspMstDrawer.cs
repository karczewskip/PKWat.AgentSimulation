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

        // Draw MST edges being built
        if (agent?.MstEdges != null && agent.MstEdges.Count > 0)
        {
            DrawMstEdges(graphic, environment, agent.MstEdges);
        }

        // Draw all TSP points
        DrawTspPoints(graphic, environment, agent);

        // Draw DFS route being built (only during DFS phase)
        if (agent?.IsDfsStarted == true && agent.CurrentRoute != null && agent.CurrentRoute.Count > 1)
        {
            DrawDfsRoute(graphic, environment, agent.CurrentRoute, agent.IsComplete);
        }

        // Draw point labels
        DrawPointLabels(graphic, environment);

        // Draw info text
        DrawInfoText(graphic, context, environment, agent);

        var bitmapSource = _bmp.ConvertToBitmapSource();
        bitmapSource.Freeze();

        return bitmapSource;
    }

    private void DrawMstEdges(Graphics graphic, TspEnvironment environment, List<(int from, int to)> edges)
    {
        using var pen = new Pen(Color.FromArgb(150, 100, 150, 255), 2);

        foreach (var (from, to) in edges)
        {
            var p1 = environment.Points[from];
            var p2 = environment.Points[to];

            graphic.DrawLine(
                pen,
                Scale * (float)p1.X,
                Scale * (float)p1.Y,
                Scale * (float)p2.X,
                Scale * (float)p2.Y);
        }
    }

    private void DrawTspPoints(Graphics graphic, TspEnvironment environment, MstAgent? agent)
    {
        foreach (var point in environment.Points)
        {
            // Color based on state:
            // - Blue/Purple if in MST but not yet visited in DFS
            // - Green if visited in DFS
            // - Orange if not yet in MST
            bool isInMst = agent?.NodesInMst?.Contains(point.Id) ?? false;
            bool isVisitedInDfs = agent?.VisitedNodes?.Contains(point.Id) ?? false;
            
            Color pointColor;
            if (isVisitedInDfs)
            {
                pointColor = Color.FromArgb(255, 100, 255, 100); // Green - visited in DFS
            }
            else if (isInMst)
            {
                pointColor = Color.FromArgb(255, 150, 150, 255); // Purple - in MST
            }
            else
            {
                pointColor = Color.FromArgb(255, 255, 200, 100); // Orange - not in MST yet
            }

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

    private void DrawDfsRoute(Graphics graphic, TspEnvironment environment, List<int> route, bool isComplete)
    {
        if (route.Count < 2)
            return;

        Color routeColor = isComplete 
            ? Color.FromArgb(200, 0, 255, 0)      // Green for complete
            : Color.FromArgb(200, 255, 255, 0);   // Yellow for in-progress

        using var pen = new Pen(routeColor, 3);

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
        if (isComplete && route.Count > 2)
        {
            var last = environment.Points[route[^1]];
            var first = environment.Points[route[0]];
            using var dashedPen = new Pen(routeColor, 3);
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            
            graphic.DrawLine(
                dashedPen,
                Scale * (float)last.X,
                Scale * (float)last.Y,
                Scale * (float)first.X,
                Scale * (float)first.Y);
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

        int mstEdgesCount = agent?.MstEdges?.Count ?? 0;
        int nodesInMst = agent?.NodesInMst?.Count ?? 0;
        int dfsNodesVisited = agent?.VisitedNodes?.Count ?? 0;
        
        string phase = !agent?.IsMstBuilt ?? true 
            ? "Building MST" 
            : (agent.IsComplete ? "Complete" : "DFS Traversal");
        
        string info = $"Algorithm: MST Approximation (Prim's)\n" +
                      $"Phase: {phase}\n" +
                      $"Iteration: {context.Time.StepNo}\n" +
                      $"Points: {environment.Points.Count}\n" +
                      $"MST Edges: {mstEdgesCount}/{environment.Points.Count - 1}\n" +
                      $"DFS Progress: {dfsNodesVisited}/{environment.Points.Count}\n" +
                      $"Best Distance: {agent?.BestSolution?.TotalDistance:F2}";

        graphic.DrawString(info, font, shadowBrush, 11, 11);
        graphic.DrawString(info, font, brush, 10, 10);
    }
}

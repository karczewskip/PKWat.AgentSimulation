namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.TspProblems;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;
using PKWat.AgentSimulation.Examples.TspProblems;
using PKWat.AgentSimulation.Examples.TspProblems.Agents;
using System.Drawing;
using System.Windows.Media.Imaging;

public class TspHeldKarpDrawer : IVisualizationDrawer
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

        var agent = context.GetAgents<HeldKarpAgent>().FirstOrDefault();

        // Draw all TSP points
        DrawTspPoints(graphic, environment);

        // Draw current state being processed (visualize the bitmask)
        if (agent != null && agent.CurrentMask > 0)
        {
            DrawCurrentState(graphic, environment, agent);
        }

        // Draw best partial/complete solution found
        if (agent?.BestSolution != null && agent.BestSolution.Route.Count > 0)
        {
            // Use different color for complete vs partial solution
            Color routeColor = agent.IsComplete 
                ? Color.FromArgb(200, 0, 255, 0)      // Green for complete
                : Color.FromArgb(200, 255, 165, 0);   // Orange for partial
            
            int routeWidth = agent.IsComplete ? 3 : 2;
            
            DrawRoute(graphic, environment, agent.BestSolution.Route, routeColor, routeWidth, agent.IsComplete);
        }

        // Draw point labels
        DrawPointLabels(graphic, environment);

        // Draw info text
        DrawInfoText(graphic, context, environment, agent);

        var bitmapSource = _bmp.ConvertToBitmapSource();
        bitmapSource.Freeze();

        return bitmapSource;
    }

    private void DrawTspPoints(Graphics graphic, TspEnvironment environment)
    {
        foreach (var point in environment.Points)
        {
            using var brush = new SolidBrush(Color.FromArgb(255, 255, 200, 100));
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

    private void DrawCurrentState(Graphics graphic, TspEnvironment environment, HeldKarpAgent agent)
    {
        // Highlight nodes in current bitmask
        for (int i = 0; i < environment.Points.Count; i++)
        {
            if ((agent.CurrentMask & (1 << i)) != 0)
            {
                var point = environment.Points[i];
                using var brush = new SolidBrush(Color.FromArgb(100, 100, 100, 255));
                
                float size = 20;
                graphic.FillEllipse(
                    brush,
                    Scale * (float)point.X - size / 2,
                    Scale * (float)point.Y - size / 2,
                    size,
                    size);
            }
        }

        // Highlight current last node
        if (agent.CurrentLastNode >= 0 && agent.CurrentLastNode < environment.Points.Count)
        {
            var lastPoint = environment.Points[agent.CurrentLastNode];
            using var brush = new SolidBrush(Color.FromArgb(150, 255, 0, 255));
            
            float size = 16;
            graphic.FillEllipse(
                brush,
                Scale * (float)lastPoint.X - size / 2,
                Scale * (float)lastPoint.Y - size / 2,
                size,
                size);
        }
    }

    private void DrawRoute(Graphics graphic, TspEnvironment environment, List<int> route, Color color, int width, bool isComplete)
    {
        if (route.Count < 2)
            return;

        using var pen = new Pen(color, width);

        for (int i = 0; i < route.Count - 1; i++)
        {
            var p1 = environment.Points[route[i]];
            var p2 = environment.Points[route[i + 1]];

            graphic.DrawLine(
                pen,
                Scale * (float)p1.X,
                Scale * (float)p1.Y,
                Scale * (float)p2.X,
                Scale * (float)p2.Y);
        }

        // Only draw return to start if the route is complete
        if (isComplete)
        {
            var last = environment.Points[route[^1]];
            var first = environment.Points[route[0]];
            graphic.DrawLine(
                pen,
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

    private void DrawInfoText(Graphics graphic, ISimulationContext context, TspEnvironment environment, HeldKarpAgent? agent)
    {
        using var font = new Font("Arial", 10, FontStyle.Bold);
        using var brush = new SolidBrush(Color.White);
        using var shadowBrush = new SolidBrush(Color.Black);

        int statesProcessed = agent?.DpTable?.Count ?? 0;
        
        string info = $"Algorithm: Held-Karp (Dynamic Programming)\n" +
                      $"Iteration: {context.Time.StepNo}\n" +
                      $"Points: {environment.Points.Count}\n" +
                      $"Current Subset Size: {agent?.CurrentSize ?? 0}\n" +
                      $"States Processed: {statesProcessed}\n" +
                      $"Best Distance: {agent?.BestSolution?.TotalDistance:F2}\n" +
                      $"Status: {(agent?.IsComplete == true ? "COMPLETE" : "Running")}";

        graphic.DrawString(info, font, shadowBrush, 11, 11);
        graphic.DrawString(info, font, brush, 10, 10);
    }
}

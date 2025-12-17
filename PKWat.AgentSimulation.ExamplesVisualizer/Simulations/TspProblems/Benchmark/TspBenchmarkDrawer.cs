namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.TspProblems.Benchmark;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;
using PKWat.AgentSimulation.Examples.TspProblems;
using PKWat.AgentSimulation.Examples.TspProblems.Benchmark;
using System;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

public class TspBenchmarkDrawer : IVisualizationDrawer
{
    private Bitmap? _bmp;
    private int _width;
    private int _height;

    public void InitializeIfNeeded(int width, int height)
    {
        if (_bmp == null || _width != width || _height != height)
        {
            _width = width;
            _height = height;
            _bmp = new Bitmap(width, height);
            _bmp.SetResolution(96, 96);
        }
    }

    public BitmapSource Draw(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspBenchmarkEnvironment>();
        var agents = context.GetAgents<TspBenchmarkAgent>().ToList();

        if (_bmp == null)
        {
            InitializeIfNeeded(1000, 800);
        }

        using (var g = Graphics.FromImage(_bmp!))
        {
            g.Clear(Color.White);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            DrawTitle(g, environment);
            DrawAgentStatus(g, agents);
            DrawCaseStatistics(g, agents, environment);
            DrawRankings(g, agents);
        }

        var bitmapSource = _bmp.ConvertToBitmapSource();
        bitmapSource.Freeze();
        return bitmapSource;
    }

    private void DrawTitle(Graphics g, TspBenchmarkEnvironment environment)
    {
        using var font = new Font("Arial", 14, FontStyle.Bold);
        using var brush = new SolidBrush(Color.Black);
        
        string title = $"TSP Algorithm Benchmark - Points: {environment.CurrentPointCount}, Example: {environment.CurrentExampleIndex + 1}/{environment.ExamplesPerRound}";
        g.DrawString(title, font, brush, 10, 10);
    }

    private void DrawAgentStatus(Graphics g, List<TspBenchmarkAgent> agents)
    {
        using var font = new Font("Arial", 10);
        using var brush = new SolidBrush(Color.Black);
        
        int y = 40;
        g.DrawString("Algorithm Status:", font, brush, 10, y);
        y += 25;
        
        foreach (var agent in agents.OrderBy(a => a.AlgorithmType))
        {
            string status = agent.HasExceededTimeLimit ? "TIMEOUT" : 
                           agent.IsComplete ? "Complete" : "Running";
            Color statusColor = agent.HasExceededTimeLimit ? Color.Red : 
                               agent.IsComplete ? Color.Green : Color.Orange;
            
            using var statusBrush = new SolidBrush(statusColor);
            
            string line = $"{agent.AlgorithmType,-15} {status,-12}";
            g.DrawString(line, font, brush, 10, y);
            
            if (!agent.HasExceededTimeLimit)
            {
                string time = $"{agent.Stopwatch.Elapsed.TotalSeconds:F3}s";
                g.DrawString(time, font, brush, 280, y);
                
                if (agent.BestSolution?.TotalDistance != double.MaxValue)
                {
                    string distance = $"Dist: {agent.BestSolution?.TotalDistance:F2}";
                    g.DrawString(distance, font, brush, 380, y);
                }
            }
            
            y += 22;
        }
    }

    private void DrawCaseStatistics(Graphics g, List<TspBenchmarkAgent> agents, TspBenchmarkEnvironment environment)
    {
        using var font = new Font("Courier New", 8);
        using var headerFont = new Font("Arial", 10, FontStyle.Bold);
        using var brush = new SolidBrush(Color.Black);
        
        int y = 160;
        g.DrawString("Statistics by Point Count (Last 20):", headerFont, brush, 10, y);
        y += 30;
        
        // Header
        string header = "Points  BruteForce              HeldKarp                MstPrim                 Best";
        g.DrawString(header, font, brush, 10, y);
        y += 18;
        
        // Draw separator
        using var pen = new Pen(Color.LightGray);
        g.DrawLine(pen, 10, y, 990, y);
        y += 5;

        // Group results by point count
        var allResults = agents.SelectMany(a => a.Results.Select(r => new { Agent = a, Result = r })).ToList();
        var pointCounts = allResults.Select(r => r.Result.PointCount).Distinct().OrderBy(p => p).ToList();

        // Take only the last 20 cases
        var displayPointCounts = pointCounts.TakeLast(20).ToList();

        foreach (var pointCount in displayPointCounts)
        {
            var resultsForPoints = allResults.Where(r => r.Result.PointCount == pointCount).ToList();
            
            // Check if all examples complete
            int completedExamples = resultsForPoints.Count / agents.Count;
            bool allComplete = completedExamples == environment.ExamplesPerRound;

            string line = $"{pointCount,3}     ";

            double? minDistance = null;
            string? bestAlgorithm = null;

            // Add stats for each algorithm
            foreach (var agent in agents.OrderBy(a => a.AlgorithmType))
            {
                var agentResults = resultsForPoints
                    .Where(r => r.Agent.AlgorithmType == agent.AlgorithmType)
                    .Select(r => r.Result)
                    .ToList();

                if (agentResults.Any())
                {
                    var completed = agentResults.Where(r => !r.ExceededTimeLimit).ToList();
                    
                    if (completed.Any())
                    {
                        double avgTime = completed.Average(r => r.ExecutionTime.TotalSeconds);
                        double avgDist = completed.Average(r => r.Distance);
                        
                        if (!minDistance.HasValue || avgDist < minDistance.Value)
                        {
                            minDistance = avgDist;
                            bestAlgorithm = agent.AlgorithmType.ToString();
                        }
                        
                        string timeStr = avgTime < 1 ? $"{avgTime * 1000:F0}ms" : $"{avgTime:F2}s";
                        line += $"{completed.Count}/{environment.ExamplesPerRound} {timeStr,7} {avgDist,6:F1}  ";
                    }
                    else
                    {
                        line += "TIMEOUT              ";
                    }
                }
                else
                {
                    line += "                     ";
                }
            }

            // Add best algorithm
            if (bestAlgorithm != null)
            {
                line += bestAlgorithm;
            }

            // Draw with color based on completion
            using var lineBrush = new SolidBrush(allComplete ? Color.DarkGreen : Color.Black);
            g.DrawString(line, font, lineBrush, 10, y);
            
            y += 16;
        }
    }

    private void DrawRankings(Graphics g, List<TspBenchmarkAgent> agents)
    {
        using var font = new Font("Arial", 9);
        using var headerFont = new Font("Arial", 10, FontStyle.Bold);
        using var brush = new SolidBrush(Color.Black);
        
        int y = 550;
        
        g.DrawString("Overall Rankings:", headerFont, brush, 10, y);
        y += 25;
        
        // Time ranking
        g.DrawString("Best Average Time:", font, brush, 10, y);
        y += 20;
        
        var timeRanking = agents
            .Where(a => !a.HasExceededTimeLimit && a.Results.Any(r => !r.ExceededTimeLimit))
            .Select(a => new
            {
                Agent = a,
                AvgTime = a.Results.Where(r => !r.ExceededTimeLimit).Average(r => r.ExecutionTime.TotalSeconds),
                CompletedTests = a.Results.Count(r => !r.ExceededTimeLimit)
            })
            .OrderBy(x => x.AvgTime)
            .ToList();

        int rank = 1;
        foreach (var item in timeRanking)
        {
            Color rankColor = rank == 1 ? Color.Goldenrod : rank == 2 ? Color.Gray : Color.SaddleBrown;
            using var rankBrush = new SolidBrush(rankColor);
            
            string text = $"{rank}. {item.Agent.AlgorithmType,-15} {item.AvgTime,8:F4}s  ({item.CompletedTests} tests)";
            g.DrawString(text, font, rankBrush, 20, y);
            y += 18;
            rank++;
        }

        y += 15;
        
        // Distance ranking
        g.DrawString("Best Average Distance:", font, brush, 10, y);
        y += 20;

        var distanceRanking = agents
            .Where(a => !a.HasExceededTimeLimit && a.Results.Any(r => !r.ExceededTimeLimit))
            .Select(a => new
            {
                Agent = a,
                AvgDistance = a.Results.Where(r => !r.ExceededTimeLimit).Average(r => r.Distance),
                CompletedTests = a.Results.Count(r => !r.ExceededTimeLimit)
            })
            .OrderBy(x => x.AvgDistance)
            .ToList();

        rank = 1;
        foreach (var item in distanceRanking)
        {
            Color rankColor = rank == 1 ? Color.Goldenrod : rank == 2 ? Color.Gray : Color.SaddleBrown;
            using var rankBrush = new SolidBrush(rankColor);
            
            string text = $"{rank}. {item.Agent.AlgorithmType,-15} {item.AvgDistance,8:F2}  ({item.CompletedTests} tests)";
            g.DrawString(text, font, rankBrush, 20, y);
            y += 18;
            rank++;
        }
    }
}

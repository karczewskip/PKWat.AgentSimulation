using PKWat.AgentSimulation.Core;

namespace PKWat.AgentSimulation.Examples.LiquidWpf.Simulation
{
    public class BinEnvironment : DefaultSimulationEnvironment
    {
        public double BinWidth { get; }
        public double BinHeight { get; }
        public HeatMap Heatmap { get; private set; }

        public DropAcceleration Gravity { get; } = new DropAcceleration(0, 9.8);

        public BinEnvironment(double binWidth, double binHeight)
        {
            BinWidth = binWidth;
            BinHeight = binHeight;
            Heatmap = HeatMap.Create(BinWidth, BinHeight, 100, 100);
        }

        public void UpdateHeatmap(Drop[] drops)
        {
            Heatmap = HeatMap.Create(BinWidth, BinHeight, 100, 100, drops.Select(d => (d.State.Position.X, d.State.Position.Y)).ToArray());
        }
    }

    public record HeatMap((int X, int Y, int Counter)[] HeatmapValues)
    {
        public static HeatMap Create(double width, double height, int rows, int columns, (double x, double y)[]? drops = null)
        {
            var heatmap = new Dictionary<(int, int), int>();
            var cellWidth = width / rows;
            var cellHeight = height / columns;

            if (drops != null)
            {
                foreach (var drop in drops)
                {
                    var x = (int)(drop.x / cellWidth);
                    var y = (int)(drop.y / cellHeight);

                    if(heatmap.ContainsKey((x, y)))
                    {
                        heatmap[(x, y)]++;
                    }
                    else
                    {
                        heatmap.Add((x, y), 1);
                    }
                }
            }

            return new HeatMap(heatmap.Select(x => (x.Key.Item1, x.Key.Item2, x.Value)).ToArray());
        }
    }

}

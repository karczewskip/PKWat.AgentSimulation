namespace PKWat.AgentSimulation.Examples.LiquidWpf.Simulation
{
    public record HeatMap((double X, double Y, int Counter)[] HeatmapValues, double CellWidth, double CellHeight)
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

            return new HeatMap(heatmap.Select(x => (x.Key.Item1*cellWidth, x.Key.Item2*cellHeight, x.Value)).ToArray(), cellWidth, cellHeight);
        }
    }

}

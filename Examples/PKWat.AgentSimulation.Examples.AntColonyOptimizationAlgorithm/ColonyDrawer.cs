namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm
{
    using System.Drawing;
    using System.Windows.Media.Imaging;
    using PKWat.AgentSimulation.Core;
    using PKWat.AgentSimulation.Drawing;

    public class ColonyDrawer
    {
        private const int AntSize = 10;
        private const int AntHillSize = 20;

        private Bitmap _bmp;

        public void Initialize(int width, int height)
        {
            _bmp = new Bitmap(width, height);
            _bmp.SetResolution(96, 96);
        }

        public BitmapSource Draw(ISimulationContext<ColonyEnvironment> context)
        {
            using var graphic = Graphics.FromImage(_bmp);
            graphic.Clear(Color.White);

            foreach(var pheromone in context.SimulationEnvironment.Pheromones)
            {
                var value = (int)((pheromone.Value <= 1 ? pheromone.Value : 1) * 255);
                var color = Color.FromArgb(value, 0, 255, 0);
                graphic.FillRectangle(new SolidBrush(color), pheromone.Key.X, pheromone.Key.Y, 1, 1);
            }

            foreach (Ant ant in context.GetAgents<Ant>())
            {
                graphic.FillEllipse(Brushes.Red, ant.Coordinates.X, ant.Coordinates.Y, AntSize, AntSize);
            }

            var anthillCoordinates = context.SimulationEnvironment.AntHill.Coordinates;
            graphic.FillEllipse(new SolidBrush(Color.FromArgb(125, 102, 51, 0)), anthillCoordinates.X, anthillCoordinates.Y, AntHillSize, AntHillSize);

            return _bmp.ConvertToBitmapSource();
        }
    }
}

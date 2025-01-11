namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation
{
    using System.Drawing;
    using System.Windows.Media.Imaging;
    using PKWat.AgentSimulation.Core;
    using PKWat.AgentSimulation.Drawing;

    public class ColonyDrawer
    {
        private const int Scale = 10;
        private const int AntSize = 1 * Scale;

        private Bitmap _bmp;

        public void Initialize(int width, int height)
        {
            _bmp = new Bitmap(Scale * width, Scale * height);
            _bmp.SetResolution(96, 96);
        }

        public BitmapSource Draw(ISimulationContext<ColonyEnvironment> context)
        {
            using var graphic = Graphics.FromImage(_bmp);
            graphic.Clear(Color.White);

            //foreach (var (coordinates, pheromone) in context.SimulationEnvironment.GetAllPheromones())
            //{
            //    var value = CalculateValue(pheromone.Food);
            //    var color = Color.FromArgb(value, 255, 0, 255);
            //    graphic.FillRectangle(new SolidBrush(color), Scale * coordinates.X, Scale * coordinates.Y, 5, 5);
            //}

            //foreach (var (coordinates, pheromone) in context.SimulationEnvironment.GetHomePheromones())
            //{
            //    var value = CalculateValue(pheromone);
            //    var color = Color.FromArgb(value, 255, 0, 255);
            //    graphic.FillRectangle(new SolidBrush(color), Scale * coordinates.X, Scale * coordinates.Y, 5, 5);
            //}

            //foreach (var (coordinates, pheromone) in context.SimulationEnvironment.GetFoodPheromones())
            //{
            //    var value = CalculateValue(pheromone);
            //    var color = Color.FromArgb(value, 255, 255, 0);
            //    graphic.FillRectangle(new SolidBrush(color), Scale * coordinates.X, Scale * coordinates.Y, 5, 5);
            //}

            foreach (var antHill in context.SimulationEnvironment.GetAntHills())
            {
                graphic.FillEllipse(
                    new SolidBrush(Color.FromArgb(125, 102, 51, 0)), 
                    Scale * antHill.Coordinates.X, 
                    Scale * antHill.Coordinates.Y, 
                    (float)(AntSize * antHill.Size),
                    (float)(AntSize * antHill.Size));
            }

            foreach (var foodSource in context.SimulationEnvironment.GetFoodSources())
            {
                graphic.FillEllipse(
                    new SolidBrush(Color.FromArgb(125, 255, 255, 0)), 
                    Scale * foodSource.Coordinates.X, 
                    Scale * foodSource.Coordinates.Y,
                    (float)(AntSize * foodSource.Size),
                    (float)(AntSize * foodSource.Size));
            }

            foreach (Ant ant in context.GetAgents<Ant>())
            {
                var brush = ant switch 
                { 
                    { IsCarryingFood: true } => Brushes.Yellow,
                    { IsAfterHillVisit: true } => Brushes.Brown,
                    _ => Brushes.Black 
                };
                graphic.FillEllipse(brush, Scale * ant.Coordinates.X, Scale * ant.Coordinates.Y, AntSize, AntSize);
            }

            var bitmapSource = _bmp.ConvertToBitmapSource();

            bitmapSource.Freeze();

            return bitmapSource;
        }

        private int CalculateValue(double value)
        {
            if (value < 0)
                return 0;

            if (value > 1)
                return 255;

            return (int)(value * 255);
        }
    }
}

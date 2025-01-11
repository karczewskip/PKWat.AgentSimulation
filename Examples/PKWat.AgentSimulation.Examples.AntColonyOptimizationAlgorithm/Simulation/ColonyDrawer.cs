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

            for(int x = 0; x < context.SimulationEnvironment.Width; x++)
            {
                for (int y = 0; y < context.SimulationEnvironment.Height; y++)
                {
                    var pheromone = context.SimulationEnvironment.Pheromones[x, y];
                    var foodValue = 255 * pheromone.Food / Pheromones.MaxPheromoneValue;
                    var homeValue = 255 * pheromone.Home / Pheromones.MaxPheromoneValue;
                    Color color;
                    if(foodValue > homeValue)
                    {
                        color = Color.FromArgb((int)foodValue, 255, 255, 0);
                    }
                    else
                    {
                        color = Color.FromArgb((int)homeValue, 255, 0, 255);
                    }
                    graphic.FillRectangle(new SolidBrush(color), Scale * x, Scale * y, AntSize, AntSize);
                }
            }

            foreach (var antHill in context.SimulationEnvironment.AntHills)
            {
                var antHillSize = (float)(AntSize * antHill.SizeRadius*2);
                var antHillX = Scale * antHill.Coordinates.X - antHillSize/2;
                var antHillY = Scale * antHill.Coordinates.Y - antHillSize/2;

                graphic.FillEllipse(
                    new SolidBrush(Color.FromArgb(125, 102, 51, 0)),
                    antHillX,
                    antHillY,
                    antHillSize,
                    antHillSize);
            }

            foreach (var foodSource in context.SimulationEnvironment.FoodSource)
            {
                var foodSourceSize = (float)(AntSize * foodSource.SizeRadius*2);
                var foodSourceX = Scale * foodSource.Coordinates.X - foodSourceSize / 2;
                var foodSourceY = Scale * foodSource.Coordinates.Y - foodSourceSize / 2;

                graphic.FillEllipse(
                    new SolidBrush(Color.FromArgb(125, 0, 51, 102)), 
                    foodSourceX,
                    foodSourceY,
                    foodSourceSize,
                    foodSourceSize);
            }

            foreach (Ant ant in context.GetAgents<Ant>())
            {
                //var brush = ant switch 
                //{ 
                //    { IsCarryingFood: true } => Brushes.Yellow,
                //    { IsAfterHillVisit: true } => Brushes.Brown,
                //    _ => Brushes.Black 
                //};

                var brush = Brushes.Black;

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

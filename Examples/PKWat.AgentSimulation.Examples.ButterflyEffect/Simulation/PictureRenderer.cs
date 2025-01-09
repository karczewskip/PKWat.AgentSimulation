namespace PKWat.AgentSimulation.Examples.ButterflyEffect.Simulation
{
    using PKWat.AgentSimulation.Core;
    using PKWat.AgentSimulation.Core.Agent;
    using PKWat.AgentSimulation.Drawing;
    using PKWat.AgentSimulation.Examples.ButterflyEffect.Simulation.Agents;
    using System.Drawing;
    using System.Windows.Media.Imaging;

    public class PictureRenderer(ColorsGenerator colorsGenerator)
    {
        private const int Scale = 1;

        private Bitmap _bmp;
        private Color[] _colors;
        private Dictionary<AgentId, Brush> _brushes;

        public void Initialize(int width, int height, AgentId[] balls)
        {
            _bmp = new Bitmap(Scale * width, Scale * height);
            _bmp.SetResolution(96, 96);

            _colors = colorsGenerator.Generate(balls.Length);
            _brushes = new Dictionary<AgentId, Brush>();
            for (int i = 0; i < balls.Length; i++)
            {
                _brushes.Add(balls[i], new SolidBrush(_colors[i % _colors.Length]));
            }
        }

        public BitmapSource Draw(ISimulationContext<BouncingBallBulb> context)
        {
            var bouncingBalls = context.GetAgents<BouncingBall>().ToArray();

            if(_brushes == null)
            {
                _brushes = new Dictionary<AgentId, Brush>();
                for(int i = 0; i < bouncingBalls.Length; i++)
                {
                    var b = bouncingBalls[i];
                    _brushes.Add(b.Id, new SolidBrush(_colors[i % _colors.Length]));
                }
            }

            using var graphic = Graphics.FromImage(_bmp);
            graphic.Clear(Color.Black);
            var pen = new Pen(Brushes.White, 0);
            graphic.DrawEllipse(pen, 0, 0, (float)context.SimulationEnvironment.BulbRadius * 2, (float)context.SimulationEnvironment.BulbRadius * 2);

            graphic.TranslateTransform(_bmp.Width / 2, _bmp.Height / 2);
            //graphic.Clear(Color.White);

            foreach (var b in bouncingBalls)
            {
                var brush = _brushes[b.Id];
                graphic.FillEllipse(brush, (int)b.Position.X, (int)b.Position.Y, (float)context.SimulationEnvironment.BallRadius, (float)context.SimulationEnvironment.BallRadius);
            }

            var bitmapSource = _bmp.ConvertToBitmapSource();

            bitmapSource.Freeze();

            return bitmapSource;

            //var imageSize = (int)radius * 2;

            //DrawingVisual drawingVisual = new DrawingVisual();
            //drawingVisual.Transform = new TranslateTransform()
            //{
            //    X = radius,
            //    Y = radius
            //};
            //using DrawingContext drawingContext = drawingVisual.RenderOpen();

            //drawingContext.DrawEllipse(Brushes.White, new Pen(Brushes.White, 1), new Point(0,0), radius, radius);

            //var pen = new Pen(Brushes.White, 0);

            //foreach (var b in bouncingBalls)
            //{
            //    drawingContext.DrawEllipse(b.State.Brush, null, new Point(b.State.X, b.State.Y), b.State.Radius, b.State.Radius);
            //}

            //drawingContext.Close();

            //var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            //var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            //var dpiX = (int)dpiXProperty.GetValue(null, null);
            //var dpiY = (int)dpiYProperty.GetValue(null, null);

            //RenderTargetBitmap bmp = new RenderTargetBitmap(imageSize, imageSize, dpiX, dpiY, PixelFormats.Pbgra32);
            //bmp.Render(drawingVisual);

            //return bmp;
        }
    }
}

namespace PKWat.AgentSimulation.Examples.ButterflyEffect
{
    using PKWat.AgentSimulation.Core;
    using PKWat.AgentSimulation.Drawing;
    using System.Drawing;
    using System.Windows.Media.Imaging;

    public class PictureRenderer
    {
        private const int Scale = 1;

        private Bitmap _bmp;

        public void Initialize(int width, int height)
        {
            _bmp = new Bitmap(Scale * width, Scale * height);
            _bmp.SetResolution(96, 96);
        }

        public BitmapSource Draw(ISimulationContext<BouncingBallBulb> context)
        {
            using var graphic = Graphics.FromImage(_bmp);
            graphic.Clear(Color.Black);
            var pen = new Pen(Brushes.White, 0);
            graphic.DrawEllipse(pen, 0, 0, (float)context.SimulationEnvironment.BulbRadius * 2, (float)context.SimulationEnvironment.BulbRadius * 2);

            graphic.TranslateTransform(_bmp.Width/2, _bmp.Height / 2);
            //graphic.Clear(Color.White);

            var bouncingBalls = context.GetAgents<BouncingBall>();
            foreach (var b in bouncingBalls)
            {
                graphic.FillEllipse(b.State.Brush, (int)b.State.Position.X, (int)b.State.Position.Y, (float)b.State.Radius, (float)b.State.Radius);
            }

            return _bmp.ConvertToBitmapSource();

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

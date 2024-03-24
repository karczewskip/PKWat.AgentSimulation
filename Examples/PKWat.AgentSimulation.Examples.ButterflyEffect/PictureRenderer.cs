namespace PKWat.AgentSimulation.Examples.ButterflyEffect
{
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class PictureRenderer
    {
        public RenderTargetBitmap Render(double radius, BouncingBall[] bouncingBalls)
        {
            var imageSize = (int)radius * 2;

            DrawingVisual drawingVisual = new DrawingVisual();
            drawingVisual.Transform = new TranslateTransform()
            {
                X = radius,
                Y = radius
            };
            using DrawingContext drawingContext = drawingVisual.RenderOpen();

            drawingContext.DrawEllipse(Brushes.White, new Pen(Brushes.White, 1), new Point(0,0), radius, radius);

            var pen = new Pen(Brushes.White, 0);

            foreach (var b in bouncingBalls)
            {
                drawingContext.DrawEllipse(b.State.Brush, null, new Point(b.State.X, b.State.Y), b.State.Radius, b.State.Radius);
            }

            drawingContext.Close();

            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = (int)dpiXProperty.GetValue(null, null);
            var dpiY = (int)dpiYProperty.GetValue(null, null);

            RenderTargetBitmap bmp = new RenderTargetBitmap(imageSize, imageSize, dpiX, dpiY, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            return bmp;
        }
    }
}

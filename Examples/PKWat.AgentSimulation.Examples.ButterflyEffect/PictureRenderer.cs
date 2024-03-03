namespace PKWat.AgentSimulation.Examples.ButterflyEffect
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    public class PictureRenderer
    {


        public RenderTargetBitmap Render(double radius, BouncingBall[] bouncingBalls)
        {
            //var circle = CreateCircle(new Point(0, 0), _radius, Brushes.White);
            //var balls = bouncingBalls.Select(x => CreateCircle(new Point(x.X, x.Y), x.Radius, x.Brush));
            //var center = CreateCircle(new Point(0, 0), 10, Brushes.Black);

            //simulationCanvas.Children.Clear();

            //simulationCanvas.Children.Add(circle);
            //foreach (var b in balls)
            //{
            //    simulationCanvas.Children.Add(b);
            //}
            //simulationCanvas.Children.Add(center);

            var imageSize = (int)radius * 2;

            DrawingVisual drawingVisual = new DrawingVisual();
            using DrawingContext drawingContext = drawingVisual.RenderOpen();

            drawingContext.DrawRectangle(Brushes.Black, new Pen(Brushes.Black, 1), new Rect(new Size(imageSize, imageSize)));
            drawingContext.DrawEllipse(Brushes.White, new Pen(Brushes.White, 1), new Point(radius, radius), radius, radius);

            foreach (var b in bouncingBalls)
            {
                drawingContext.DrawEllipse(b.Brush, new Pen(b.Brush, 1), new Point(radius + b.X, radius + b.Y), b.Radius, b.Radius);
            }

            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap(imageSize, imageSize, 100, 100, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            return bmp;
        }

        //private Path CreateCircle(Point center, double radius, Brush brush)
        //{
        //    var circle = new Path();
        //    var circleGeometry = new PathGeometry();
        //    var ellipseGeometry = new EllipseGeometry();
        //    ellipseGeometry.Center = center;
        //    ellipseGeometry.RadiusX = radius;
        //    ellipseGeometry.RadiusY = radius;
        //    circleGeometry.AddGeometry(ellipseGeometry);

        //    circle.Data = circleGeometry;
        //    circle.Fill = brush;
        //    circle.RenderTransform = _centerTransform;

        //    return circle;
        //}
    }
}

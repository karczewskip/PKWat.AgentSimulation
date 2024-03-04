namespace PKWat.AgentSimulation.Examples.ButterflyEffect
{
    using System.Drawing;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class PictureRenderer
    {
        private int _imageSize;
        private double _radius;
        private DrawingVisual _drawingVisual = new DrawingVisual();
        private DrawingContext _drawingContext;
        private System.Drawing.Bitmap _bmp;
        private Graphics g;

        public void Initialize(double radius)
        {
            _imageSize = (int)radius * 2;
            _radius = radius;

            _drawingVisual.Transform = new TranslateTransform()
            {
                X = radius,
                Y = radius
            };
            _bmp = new System.Drawing.Bitmap(
                _imageSize,
                _imageSize
            );
            g = Graphics.FromImage(_bmp);
            _drawingContext = _drawingVisual.RenderOpen();
        }

        public BitmapSource Render(BouncingBall[] bouncingBalls)
        {
            g.FillEllipse(System.Drawing.Brushes.White, new Rectangle(0, 0, _imageSize, _imageSize));

            foreach (var b in bouncingBalls)
            {
                g.FillEllipse(System.Drawing.Brushes.Red, (float)(_radius + b.X), (float)(_radius + b.Y), (float)b.Radius, (float)b.Radius);
                //_drawingContext.DrawEllipse(b.Brush, null, new Point(b.X, b.Y), b.Radius, b.Radius);
            }

            System.Windows.Media.Imaging.BitmapSource bitmapSource =
              System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
              _bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
              System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            return bitmapSource;

            //_drawingContext = _drawingVisual.RenderOpen();
            //_drawingContext.DrawEllipse(Brushes.White, null, new Point(0,0), _radius, _radius);

            //foreach (var b in bouncingBalls)
            //{
            //    _drawingContext.DrawEllipse(b.Brush, null, new Point(b.X, b.Y), b.Radius, b.Radius);
            //}

            //var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            //var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            //var dpiX = (int)dpiXProperty.GetValue(null, null);
            //var dpiY = (int)dpiYProperty.GetValue(null, null);

            //_bmp.Clear();
            //_bmp.Render(_drawingVisual);
            //writableBitmap.
            //_drawingContext.Close();

            //return _bmp;
        }
    }
}

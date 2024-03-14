namespace PKWat.AgentSimulations.Examples.CollisionDetection;

using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;

public class BallsContainerDrawer
{
    private const double BallRadius = 10;

    private Bitmap _bmp;
    private double _xScale;
    private double _yScale;
    private double _maxHeight = 0;
    private double _height = 0;
    private double _lastVelocity = 0;

    public void Initialize(int bitmapWidth, int bitmapHeight, double width, double height)
    {
        _bmp = new Bitmap(bitmapWidth, bitmapHeight);
        _bmp.SetResolution(96, 96);

        _xScale = bitmapWidth / width;
        _yScale = bitmapHeight / height;
    }

    public BitmapSource Draw(ISimulationContext<BallsContainer> context)
    {
        using var graphic = Graphics.FromImage(_bmp);
        graphic.ScaleTransform(1.0F, -1.0F);
        graphic.TranslateTransform(0.0F, -(float)_bmp.Height);

        graphic.Clear(Color.White);
        var velocity = 0.0;
        foreach (var ball in context.GetAgents<Ball>())
        {
            graphic.FillEllipse(
                Brushes.Black,
                (float)(ball.Coordinates.X * _xScale),
                (float)(ball.Coordinates.Y * _yScale),
                (float)(BallRadius ),
                (float)(BallRadius ));

            velocity = ball.Velocity.Y;
            if(_maxHeight < ball.Coordinates.Y)
            {
                _maxHeight = ball.Coordinates.Y;
            }
            if(_lastVelocity > 0 && ball.Velocity.Y <= 0)
            {
                _height = ball.Coordinates.Y;
            }
            _lastVelocity = ball.Velocity.Y;

        }

        graphic.ScaleTransform(1.0F, -1.0F);
        graphic.TranslateTransform(0.0F, -(float)_bmp.Height);

        graphic.DrawString($"{velocity}", new Font("Arial", 16), Brushes.Black, new PointF(_bmp.Width / 2, _bmp.Height / 2));
        graphic.DrawString($"{context.SimulationTime}", new Font("Arial", 16), Brushes.Black, new PointF(_bmp.Width / 2, _bmp.Height / 2 + 20));
        graphic.DrawString($"{_maxHeight}", new Font("Arial", 16), Brushes.Black, new PointF(_bmp.Width / 2, _bmp.Height / 2 + 40));
        graphic.DrawString($"{_height}", new Font("Arial", 16), Brushes.Black, new PointF(_bmp.Width / 2, _bmp.Height / 2 + 60));

        return _bmp.ConvertToBitmapSource();
    }


}

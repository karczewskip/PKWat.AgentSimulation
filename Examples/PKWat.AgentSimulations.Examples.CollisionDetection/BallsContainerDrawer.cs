namespace PKWat.AgentSimulations.Examples.CollisionDetection;

using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;

public class BallsContainerDrawer
{
    private const double BallKernelRadius = 2;

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
        for(double i = 0.1;  i <= 1; i+= 0.5)
        {
            DrawBalls(context, graphic, i);
        }
        foreach (var ball in context.GetAgents<Ball>())
        {
            //graphic.FillEllipse(
            //    new SolidBrush(ball.Color),
            //    (float)(ball.Coordinates.X * _xScale - ball.Radius / 2),
            //    (float)(ball.Coordinates.Y * _yScale - ball.Radius / 2),
            //    (float)(ball.Radius),
            //    (float)(ball.Radius));

            //graphic.FillEllipse(
            //    Brushes.Black,
            //    (float)(ball.Coordinates.X * _xScale),
            //    (float)(ball.Coordinates.Y * _yScale),
            //    (float)(BallKernelRadius),
            //    (float)(BallKernelRadius));

            velocity = ball.State.Velocity.Y;
            if (_maxHeight < ball.State.Coordinates.Y)
            {
                _maxHeight = ball.State.Coordinates.Y;
            }
            if (_lastVelocity > 0 && ball.State.Velocity.Y <= 0)
            {
                _height = ball.State.Coordinates.Y;
            }
            _lastVelocity = ball.State.Velocity.Y;

        }

        graphic.ScaleTransform(1.0F, -1.0F);
        graphic.TranslateTransform(0.0F, -(float)_bmp.Height);

        graphic.DrawString($"{velocity}", new Font("Arial", 16), Brushes.Black, new PointF(_bmp.Width / 2, _bmp.Height / 2));
        graphic.DrawString($"{context.SimulationTime}", new Font("Arial", 16), Brushes.Black, new PointF(_bmp.Width / 2, _bmp.Height / 2 + 20));
        graphic.DrawString($"{_maxHeight}", new Font("Arial", 16), Brushes.Black, new PointF(_bmp.Width / 2, _bmp.Height / 2 + 40));
        graphic.DrawString($"{_height}", new Font("Arial", 16), Brushes.Black, new PointF(_bmp.Width / 2, _bmp.Height / 2 + 60));

        return _bmp.ConvertToBitmapSource();
    }

    private void DrawBalls(ISimulationContext<BallsContainer> context, Graphics graphic, double scale)
    {
        foreach (var ball in context.GetAgents<Ball>())
        {
            var radius = context.SimulationEnvironment.GetBallRadius() * 10;
            graphic.FillEllipse(
                new SolidBrush(Color.FromArgb((int)((1-scale)*255), ball.State.Color)),
                (float)(ball.State.Coordinates.X * _xScale - radius * scale),
                (float)(ball.State.Coordinates.Y * _yScale - radius * scale),
                (float)(radius * 2 * scale),
                (float)(radius * 2 * scale));

        }
    }

}

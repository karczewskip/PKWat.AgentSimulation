namespace PKWat.AgentSimulation.Examples.ButterflyEffect;

using PKWat.AgentSimulation.Core;
using System.Windows.Media;

public class BouncingBall : IAgent
{
    private readonly double _maxDistanceFromCenter;
    private readonly double _gravity;

    public BouncingBall(
        double startX,
        double startY,
        double radius,
        double startDeltaX,
        double startDeltaY,
        double maxDistanceFromCenter,
        double gravity,
        Brush brush)
    {
        X = startX;
        Y = startY;
        Radius = radius;
        DeltaX = startDeltaX;
        DeltaY = startDeltaY;
        Brush = brush;
        _maxDistanceFromCenter = maxDistanceFromCenter;
        _gravity = gravity;
    }

    public double X { get; private set; } = 0;
    public double Y { get; private set; } = 0;

    public double DeltaX { get; private set; } = 0;
    public double DeltaY { get; private set; } = 1;

    public double Radius { get; private set; } = 10;
    public Brush Brush { get; }

    public double VelocityRadian => Math.Atan2(DeltaY, DeltaX);
    public double VelocityDirection => Math.Tan(VelocityRadian);
    public double NormalRadian => Math.Atan2(Y, X);
    public double NormalDirection => Math.Tan(NormalRadian);

    public void Act()
    {
        DeltaY += _gravity;
        var newX = X + DeltaX;
        var newY = Y + DeltaY;
        if (_maxDistanceFromCenter > Math.Sqrt(newX * newX + newY * newY) + Radius)
        {
            X = newX;
            Y = newY;
        }
        else
        {
            double newDirectionRadian = 2 * NormalRadian - VelocityRadian;
            double speed = Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);
            DeltaX = -Math.Cos(newDirectionRadian) * speed;
            DeltaY = -Math.Sin(newDirectionRadian) * speed;
        }
    }
}
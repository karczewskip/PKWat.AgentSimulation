namespace PKWat.AgentSimulation.Examples.ButterflyEffect;

using PKWat.AgentSimulation.Core;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

public class BouncingBallBulb
{

}

public class BouncingBall : IAgent<BouncingBallBulb>
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

    public void Decide(BouncingBallBulb simulationEnvironment)
    {
        throw new NotImplementedException();
    }

    public void Act(BouncingBallBulb simulationEnvironment)
    {
        DeltaY += _gravity;
        X = X + DeltaX;
        Y = Y + DeltaY;
        var distanceFromCenter = Math.Sqrt(X * X + Y * Y);
        var distanceExceeded = distanceFromCenter + Radius - _maxDistanceFromCenter;

        if (distanceExceeded > 0)
        {
            var previousVelocityRadian = VelocityRadian;
            double newDirectionRadian = 2 * NormalRadian - VelocityRadian;
            double speed = Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);
            DeltaX = -Math.Cos(newDirectionRadian) * speed;
            DeltaY = -Math.Sin(newDirectionRadian) * speed;

            X = X - distanceExceeded * (Math.Cos(previousVelocityRadian) + Math.Cos(newDirectionRadian));
            Y = Y - distanceExceeded * (Math.Sin(previousVelocityRadian) + Math.Sin(newDirectionRadian));

            //var ek = 0.5 * speed * speed;
            //var ep = _gravity * (_maxDistanceFromCenter - Y);
            //var et = ek + ep;
            //Debug.WriteLine($"Bounced with speed {speed}, energy: {et} = {ek} + {ep}");
        }
    }
}
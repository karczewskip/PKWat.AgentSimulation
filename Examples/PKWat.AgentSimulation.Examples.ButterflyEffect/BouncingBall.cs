namespace PKWat.AgentSimulation.Examples.ButterflyEffect;

using PKWat.AgentSimulation.Core;
using System.Diagnostics;
using System.Windows;
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

    private double _distanceFromCenter;
    private double _distanceExceeded;
    private double _previousVelocityRadian;
    private double _newDirectionRadian;
    private double _speed;

    public void Act()
    {
        DeltaY += _gravity;
        X = X + DeltaX;
        Y = Y + DeltaY;
        _distanceFromCenter = Math.Sqrt(X * X + Y * Y);
        _distanceExceeded = _distanceFromCenter + Radius - _maxDistanceFromCenter;

        if (_distanceExceeded > 0)
        {
            _previousVelocityRadian = VelocityRadian;
            _newDirectionRadian = 2 * NormalRadian - _previousVelocityRadian;
            _speed = Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);
            DeltaX = -Math.Cos(_newDirectionRadian) * _speed;
            DeltaY = -Math.Sin(_newDirectionRadian) * _speed;

            X = X - _distanceExceeded * (Math.Cos(_previousVelocityRadian) + Math.Cos(_newDirectionRadian));
            Y = Y - _distanceExceeded * (Math.Sin(_previousVelocityRadian) + Math.Sin(_newDirectionRadian));

            //var ek = 0.5 * speed * speed;
            //var ep = _gravity * (_maxDistanceFromCenter - Y);
            //var et = ek + ep;
            //Debug.WriteLine($"Bounced with speed {speed}, energy: {et} = {ek} + {ep}");
        }
    }
}
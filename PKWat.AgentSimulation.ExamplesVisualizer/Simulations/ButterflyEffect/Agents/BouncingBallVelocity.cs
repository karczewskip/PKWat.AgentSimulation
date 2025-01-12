namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.ButterflyEffect.Agents;
using System;

public class BouncingBallVelocity
{
    public double X { get; private set; }
    public double Y { get; private set; }

    public double Radian => Math.Atan2(Y, X);
    public double Speed => Math.Sqrt(X * X + Y * Y);

    private BouncingBallVelocity(double x, double y)
    {
        X = x; Y = y;
    }

    public static BouncingBallVelocity CreateStopped()
    {
        return new BouncingBallVelocity(0, 0);
    }

    public void Accelerate(double deltaX, double deltaY)
    {
        X += deltaX;
        Y += deltaY;
    }

    public void ChangeDirection(double newRadian)
    {
        var speed = Speed;
        X = -Math.Cos(newRadian) * speed;
        Y = -Math.Sin(newRadian) * speed;
    }
}

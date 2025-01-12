namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.ButterflyEffect.Agents;
using System;

public class BouncingBallPosition
{
    public double X { get; private set; }
    public double Y { get; private set; }

    public double NormalRadian => Math.Atan2(Y, X);
    public double DistanceFromCenter => Math.Sqrt(X * X + Y * Y);

    private BouncingBallPosition(double x, double y)
    {
        X = x; Y = y;
    }

    public static BouncingBallPosition CreateInCenter()
    {
        return new BouncingBallPosition(0, 0);
    }

    public void Move(double delatX, double deltaY)
    {
        X += delatX;
        Y += deltaY;
    }

    public void Move(BouncingBallVelocity velocity, double scale = 1)
    {
        X += velocity.X * scale;
        Y += velocity.Y * scale;
    }
}

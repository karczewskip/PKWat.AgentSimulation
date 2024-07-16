namespace PKWat.AgentSimulation.Examples.ButterflyEffect;

using PKWat.AgentSimulation.Core;
using System;
using System.Drawing;

public class BouncingBallBulb
{
    public double BulbRadius { get;}
    public double BallRadius { get; }
    public double Gravity => 0.25;

    public BouncingBallBulb(double bulbRadius, double ballRadius)
    {
        BulbRadius = bulbRadius;
        BallRadius = ballRadius;
    }
}

public record BouncingBallPosition(double X, double Y)
{
    public double NormalRadian => Math.Atan2(Y, X);
    public double DistanceFromCenter => Math.Sqrt(X * X + Y * Y);
}

public record BouncingBallVelocity(double X, double Y)
{
    public double Radian => Math.Atan2(Y, X);
    public double Speed => Math.Sqrt(X * X + Y * Y);
}

public record BouncingBallState(BouncingBallPosition Position, BouncingBallVelocity Velocity, double Radius, Brush Brush);

public class BouncingBallStateInitializer
{
    private int _created = 0;
    private Color[] _colors;
    private int _allBallsNumber;

    public void Initialize(Color[] colors, int allBallsNumber)
    {
        _colors = colors;
        _allBallsNumber = allBallsNumber;
    }

    public BouncingBallState InitializeNewState(BouncingBallBulb environment)
    {
        var ballNumber = _created++;
        var startX = ((_allBallsNumber / 2 - ballNumber) * 0.00001) / _allBallsNumber;
        var startY = 0;
        var radius = environment.BallRadius;
        var brush = new SolidBrush(_colors[ballNumber%_allBallsNumber]);

        return new BouncingBallState(new BouncingBallPosition(startX, startY), new BouncingBallVelocity(0, 0), radius, brush);
    }

}

public class BouncingBall : SimulationAgent<BouncingBallBulb, BouncingBallState>
{
    private readonly BouncingBallStateInitializer _bouncingBallStateInitializer;

    public BouncingBall(BouncingBallStateInitializer bouncingBallStateInitializer)
    {
        _bouncingBallStateInitializer = bouncingBallStateInitializer;
    }

    protected override BouncingBallState GetInitialState(BouncingBallBulb environment)
    {
        return _bouncingBallStateInitializer.InitializeNewState(environment);
    }

    protected override BouncingBallState GetNextState(BouncingBallBulb environment, SimulationTime simulationTime)
    {
        var newPosition = State.Position with { X = State.Position.X + State.Velocity.X, Y = State.Position.Y + State.Velocity.Y };
        var distanceExceeded = newPosition.DistanceFromCenter + State.Radius - environment.BulbRadius;

        if (distanceExceeded > 0)
        {
            var previousVelocityRadian = State.Velocity.Radian;
            double newDirectionRadian = 2 * State.Position.NormalRadian - previousVelocityRadian;
            var afterBounceSpeed = State.Velocity.Speed * 0.99;
            var newVelocity = State.Velocity with
            {
                X = -Math.Cos(newDirectionRadian) * afterBounceSpeed,
                Y = -Math.Sin(newDirectionRadian) * afterBounceSpeed
            };

            newPosition = State.Position with
            {
                X = State.Position.X - distanceExceeded * (Math.Cos(previousVelocityRadian) + Math.Cos(newDirectionRadian)),
                Y = State.Position.Y - distanceExceeded * (Math.Sin(previousVelocityRadian) + Math.Sin(newDirectionRadian))
            };

            return State with
            {
                Position = newPosition,
                Velocity = newVelocity
            };
        }
        else
        {
            var newVelocity = State.Velocity with { Y = State.Velocity.Y + environment.Gravity };

            return State with
            {
                Position = newPosition,
                Velocity = newVelocity
            };
        }

    }
}
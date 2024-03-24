namespace PKWat.AgentSimulation.Examples.ButterflyEffect;

using PKWat.AgentSimulation.Core;
using System;
using System.Windows.Media;

public class BouncingBallBulb
{
    public double BulbRadius => 400.0;
    public double BallRadius => 5.0;
    public double Gravity => 0.25;
}

public record BouncingBallState(double X, double Y, double DeltaX, double DeltaY, double Radius, Brush Brush)
{
    public double VelocityRadian => Math.Atan2(DeltaY, DeltaX);
    public double VelocityDirection => Math.Tan(VelocityRadian);
    public double NormalRadian => Math.Atan2(Y, X);
    public double NormalDirection => Math.Tan(NormalRadian);
}

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
        var startY = -(environment.BulbRadius / 2);
        var startDeltaX = 0;
        var startDeltaY = 0;
        var radius = environment.BallRadius;
        var brush = new SolidColorBrush(_colors[ballNumber]);

        return new BouncingBallState(startX, startY, startDeltaX, startDeltaY, radius, brush);
    }

}

public class BouncingBall : SimulationAgent<BouncingBallBulb, BouncingBallState>
{
    private readonly BouncingBallStateInitializer _bouncingBallStateInitializer;

    public BouncingBall(BouncingBallStateInitializer bouncingBallStateInitializer)
    {
        _bouncingBallStateInitializer = bouncingBallStateInitializer;
    }

    public void Act(ISimulationContext<BouncingBallBulb> simulationContext)
    {
    }

    protected override BouncingBallState GetInitialState(ISimulationContext<BouncingBallBulb> simulationContext)
    {
        return _bouncingBallStateInitializer.InitializeNewState(simulationContext.SimulationEnvironment);
    }

    protected override BouncingBallState GetNextState(ISimulationContext<BouncingBallBulb> simulationContext)
    {
        var environment = simulationContext.SimulationEnvironment;
        var newDeltaX = State.DeltaX;
        var newDeltaY = State.DeltaY + environment.Gravity;
        var newX = State.X + State.DeltaX;
        var newY = State.Y + State.DeltaY;
        var distanceFromCenter = Math.Sqrt(newX * newX + newY * newY);
        var distanceExceeded = distanceFromCenter + State.Radius - environment.BulbRadius;

        if (distanceExceeded > 0)
        {
            var previousVelocityRadian = State.VelocityRadian;
            double newDirectionRadian = 2 * State.NormalRadian - State.VelocityRadian;
            double speed = Math.Sqrt(newDeltaX * newDeltaX + newDeltaY * newDeltaY);
            newDeltaX = -Math.Cos(newDirectionRadian) * speed;
            newDeltaY = -Math.Sin(newDirectionRadian) * speed;

            newX = newX - distanceExceeded * (Math.Cos(previousVelocityRadian) + Math.Cos(newDirectionRadian));
            newY = newY - distanceExceeded * (Math.Sin(previousVelocityRadian) + Math.Sin(newDirectionRadian));
        }

        return State with
        {
            X = newX,
            Y = newY,
            DeltaX = newDeltaX,
            DeltaY = newDeltaY
        };
    }
}
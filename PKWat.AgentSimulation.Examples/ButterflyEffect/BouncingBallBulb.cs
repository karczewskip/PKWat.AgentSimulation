namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.ButterflyEffect;

using PKWat.AgentSimulation.Core.Environment;
using System;

public class BouncingBallBulb : DefaultSimulationEnvironment
{
    public double BulbRadius { get; private set; } = 400;
    public double BallRadius { get; set; } = 10;
    public double Gravity { get; set; } = 0.25;

    public void UseSize(double buldRadius, double ballRadius, double gravity)
    {
        BulbRadius = buldRadius;
        BallRadius = ballRadius;
        Gravity = gravity;
    }
}

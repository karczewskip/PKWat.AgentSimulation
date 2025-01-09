namespace PKWat.AgentSimulation.Examples.ButterflyEffect.Simulation.Agents;

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Examples.ButterflyEffect.Simulation;

public class BouncingBall : SimpleSimulationAgent<BouncingBallBulb>
{
    public BouncingBallPosition Position { get; private set; } = BouncingBallPosition.CreateInCenter();
    public BouncingBallVelocity Velocity { get; private set; } = BouncingBallVelocity.CreateStopped();
}
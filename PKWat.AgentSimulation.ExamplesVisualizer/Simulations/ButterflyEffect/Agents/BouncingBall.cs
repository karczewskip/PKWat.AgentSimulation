namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.ButterflyEffect.Agents;

using PKWat.AgentSimulation.Core.Agent;

public class BouncingBall : SimpleSimulationAgent
{
    public BouncingBallPosition Position { get; private set; } = BouncingBallPosition.CreateInCenter();
    public BouncingBallVelocity Velocity { get; private set; } = BouncingBallVelocity.CreateStopped();
}
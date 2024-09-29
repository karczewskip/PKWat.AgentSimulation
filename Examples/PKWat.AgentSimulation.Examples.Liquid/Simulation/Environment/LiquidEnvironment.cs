namespace PKWat.AgentSimulation.Examples.Liquid.Simulation.Environment
{
    using PKWat.AgentSimulation.Core;

    internal class LiquidEnvironment : ISimulationEnvironment
    {
        public object CreateSnapshot()
        {
            return new { SnapshotProp = "test" };
        }
    }
}

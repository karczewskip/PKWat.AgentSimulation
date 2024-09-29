namespace PKWat.AgentSimulation.Core
{
    using PKWat.AgentSimulation.Core.Snapshots;

    public interface ISimulationEnvironment : ISnapshotCreator
    {
    }

    public class DefaultSimulationEnvironment : ISimulationEnvironment
    {
        public object CreateSnapshot()
        {
            return this;
        }
    }
}
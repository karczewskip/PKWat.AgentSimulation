namespace PKWat.AgentSimulation.Core
{
    using PKWat.AgentSimulation.Core.Snapshots;

    public interface ISimulationEnvironment<SIMULATION_STATE> : ISnapshotCreator
    {
        void LoadState(SIMULATION_STATE state);
        void UpdateState();
    }

    public abstract class DefaultSimulationEnvironment<SIMULATION_STATE> : ISimulationEnvironment<SIMULATION_STATE>
    {
        private SIMULATION_STATE _state;

        public void LoadState(SIMULATION_STATE state)
        {
            _state = state;
        }

        public abstract void UpdateState();

        public object CreateSnapshot()
        {
            return this;
        }
    }
}
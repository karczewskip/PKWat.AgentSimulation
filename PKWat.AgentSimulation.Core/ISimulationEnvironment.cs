namespace PKWat.AgentSimulation.Core
{
    using PKWat.AgentSimulation.Core.Snapshots;

    public interface ISimulationEnvironment
    {
        SimulationCrashResult CheckCrashConditions();
    }

    public interface ISimulationEnvironment<SIMULATION_STATE> : ISimulationEnvironment, ISnapshotCreator
    {
        void LoadState(SIMULATION_STATE state);
    }

    public abstract class DefaultSimulationEnvironment<SIMULATION_STATE> : ISimulationEnvironment<SIMULATION_STATE>
    {
        private SIMULATION_STATE _state;

        public void LoadState(SIMULATION_STATE state)
        {
            _state = state;
        }

        protected SIMULATION_STATE GetState()
        {
            return _state;
        }

        public virtual SimulationCrashResult CheckCrashConditions()
        {
            return SimulationCrashResult.NoCrash;
        }

        public virtual object CreateSnapshot()
        {
            return _state;
        }
    }
}
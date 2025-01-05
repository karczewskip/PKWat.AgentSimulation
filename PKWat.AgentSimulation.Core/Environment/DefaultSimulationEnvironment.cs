namespace PKWat.AgentSimulation.Core.Environment
{
    using PKWat.AgentSimulation.Core.Crash;

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
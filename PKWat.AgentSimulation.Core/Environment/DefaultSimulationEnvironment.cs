namespace PKWat.AgentSimulation.Core.Environment
{
    using PKWat.AgentSimulation.Core.Crash;

    public abstract class DefaultSimulationEnvironment : ISimulationEnvironment
    {
        public virtual SimulationCrashResult CheckCrashConditions()
        {
            return SimulationCrashResult.NoCrash;
        }

        public virtual object CreateSnapshot()
        {
            return new { };
        }
    }
}
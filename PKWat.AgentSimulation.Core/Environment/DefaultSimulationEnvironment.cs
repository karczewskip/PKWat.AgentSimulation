namespace PKWat.AgentSimulation.Core.Environment
{
    public abstract class DefaultSimulationEnvironment : ISimulationEnvironment
    {
        public virtual object CreateSnapshot()
        {
            return new { };
        }
    }
}
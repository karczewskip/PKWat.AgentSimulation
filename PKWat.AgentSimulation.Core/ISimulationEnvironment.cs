namespace PKWat.AgentSimulation.Core
{
    public interface ISimulationEnvironment
    {

    }

    public interface ISimulationEnvironment<SIMULATION_STATE> : ISimulationEnvironment
    {
        void Update(SIMULATION_STATE state);
    }
}
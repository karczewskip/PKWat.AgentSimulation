namespace PKWat.AgentSimulation.Core
{
    public interface ISimulation
    {
        void Start(Action act);
    }

    public class Simulation : ISimulation
    {
        public void Start(Action act)
        {
            act();
        }
    }
}

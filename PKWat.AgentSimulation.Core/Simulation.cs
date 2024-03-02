namespace PKWat.AgentSimulation.Core
{
    public interface ISimulation
    {
        Task StartAsync();
    }

    internal class Simulation : ISimulation
    {
        private readonly SimulationContext _context;

        public Simulation(SimulationContext context)
        {
            _context = context;
        }

        public async Task StartAsync()
        {
            while (true)
            {
                await Parallel.ForEachAsync(
                    _context.Agents, 
                    (x, c) => new ValueTask(Task.Run( () => x.Act())));

                foreach (var callback in _context.Callbacks)
                {
                    await callback();
                }

                await Task.Delay(_context.WaitingTimeBetweenSteps);
            }
        }
    }
}
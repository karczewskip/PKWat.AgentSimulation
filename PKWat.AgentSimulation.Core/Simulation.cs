namespace PKWat.AgentSimulation.Core
{
    public interface ISimulation
    {
        Task StartAsync();
        Task StopAsync();
    }

    internal class Simulation : ISimulation
    {
        private bool _stopped = false;

        private readonly SimulationContext _context;

        public Simulation(SimulationContext context)
        {
            _context = context;
        }

        public async Task StartAsync()
        {
            while (_stopped is false)
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

        public async Task StopAsync()
        {
            _stopped = true;
        }
    }
}
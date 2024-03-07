namespace PKWat.AgentSimulation.Core
{
    public interface ISimulation
    {
        public bool Running { get; }

        Task StartAsync();
        Task StopAsync();
    }

    internal class Simulation : ISimulation
    {
        private readonly SimulationContext _context;
        private readonly IReadOnlyList<Func<SimulationContext, Task>> _callbacks;

        public bool Running { get; private set; } = false;

        public Simulation(SimulationContext context, IReadOnlyList<Func<SimulationContext,Task>> callbacks)
        {
            _context = context;
            _callbacks = callbacks;
        }

        public async Task StartAsync()
        {
            Running = true;

            while (Running)
            {
                await Parallel.ForEachAsync(
                    _context.Agents, 
                    new ParallelOptions() { MaxDegreeOfParallelism = 2 },
                    (x, c) => new ValueTask(Task.Run( () => x.Act())));

                foreach (var callback in _callbacks)
                {
                    await callback(_context);
                }

                await Task.Delay(_context.WaitingTimeBetweenSteps);
            }
        }

        public async Task StopAsync()
        {
            Running = false;
        }
    }
}
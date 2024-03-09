namespace PKWat.AgentSimulation.Core
{
    public interface ISimulation
    {
        public bool Running { get; }

        Task StartAsync();
        Task StopAsync();
    }

    internal class Simulation<T> : ISimulation
    {
        private readonly SimulationContext<T> _context;
        private readonly IReadOnlyList<Func<SimulationContext<T>, Task>> _environmentUpdates;
        private readonly IReadOnlyList<Func<SimulationContext<T>, Task>> _callbacks;

        public bool Running { get; private set; } = false;

        public Simulation(SimulationContext<T> context, IReadOnlyList<Func<SimulationContext<T>, Task>> environmentUpdates, IReadOnlyList<Func<SimulationContext<T>, Task>> callbacks)
        {
            _context = context;
            _environmentUpdates = environmentUpdates;
            _callbacks = callbacks;
        }

        public async Task StartAsync()
        {
            Running = true;

            await Parallel.ForEachAsync(
                    _context.Agents,
                    new ParallelOptions() { MaxDegreeOfParallelism = 2 },
                    (x, c) => new ValueTask(Task.Run(() => x.Initialize(_context.SimulationEnvironment))));

            while (Running)
            {
                foreach (var environmentUpdate in _environmentUpdates)
                {
                    await environmentUpdate(_context);
                }

                await Parallel.ForEachAsync(
                    _context.Agents,
                    new ParallelOptions() { MaxDegreeOfParallelism = 2 },
                    (x, c) => new ValueTask(Task.Run(() => x.Decide(_context.SimulationEnvironment))));

                await Parallel.ForEachAsync(
                    _context.Agents, 
                    new ParallelOptions() { MaxDegreeOfParallelism = 2 },
                    (x, c) => new ValueTask(Task.Run( () => x.Act(_context.SimulationEnvironment))));

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
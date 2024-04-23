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
        private readonly IReadOnlyList<ISimulationEvent<T>> _events;

        public bool Running { get; private set; } = false;

        public Simulation(
            SimulationContext<T> context, 
            IReadOnlyList<Func<SimulationContext<T>, Task>> environmentUpdates,
            IReadOnlyList<Func<SimulationContext<T>, Task>> callbacks,
            IReadOnlyList<ISimulationEvent<T>> events)
        {
            _context = context;
            _environmentUpdates = environmentUpdates;
            _callbacks = callbacks;
            _events = events;
        }

        public async Task StartAsync()
        {
            Running = true;

            await Parallel.ForEachAsync(
                    _context.Agents,
                    new ParallelOptions() { MaxDegreeOfParallelism = 2 },
                    (x, c) => new ValueTask(Task.Run(() => x.Initialize(_context))));

            while (Running)
            {
                foreach (var environmentUpdate in _environmentUpdates)
                {
                    await environmentUpdate(_context);
                }

                foreach (var simulationEvent in _events)
                {
                    if(await simulationEvent.ShouldBeExecuted(_context))
                    {
                        await simulationEvent.Execute(_context);
                    }
                }

                await Parallel.ForEachAsync(
                    _context.Agents,
                    new ParallelOptions() { MaxDegreeOfParallelism = 2 },
                    (x, c) => new ValueTask(Task.Run(() => x.Prepare(_context))));

                await Parallel.ForEachAsync(
                    _context.Agents, 
                    new ParallelOptions() { MaxDegreeOfParallelism = 2 },
                    (x, c) => new ValueTask(Task.Run( () => x.Act())));

                foreach (var callback in _callbacks)
                {
                    await callback(_context);
                }

                _context.Update();
                await Task.Delay(_context.WaitingTimeBetweenSteps);
            }
        }

        public async Task StopAsync()
        {
            Running = false;
        }
    }
}
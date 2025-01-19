namespace PKWat.AgentSimulation.Core
{
    using PKWat.AgentSimulation.Core.Crash;

    internal class RunningSimulationState
    {
        private CancellationTokenSource? _cancellationTokenSource;

        public bool IsRunning { get; private set; }
        public CancellationToken CancellationToken { get; private set; }
        public SimulationCrashResult CrashResult { get; private set; } = SimulationCrashResult.NoCrash;

        private RunningSimulationState(
            CancellationTokenSource? cancellationTokenSource,
            bool isRunning,
            CancellationToken cancellationToken)
        {
            _cancellationTokenSource = cancellationTokenSource;

            IsRunning = isRunning;
            CancellationToken = cancellationToken;
        }

        public static RunningSimulationState CreateRunningState()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            return new RunningSimulationState(cancellationTokenSource, true, cancellationToken);
        }

        public static RunningSimulationState CreateNotRunningState()
        {
            return new RunningSimulationState(null, false, CancellationToken.None);
        }

        public void Stop()
        {
            IsRunning = false;
            _cancellationTokenSource?.Cancel();
        }

        public void Crash(SimulationCrashResult crash)
        {
            CrashResult = crash;
            Stop();
        }
    }
}
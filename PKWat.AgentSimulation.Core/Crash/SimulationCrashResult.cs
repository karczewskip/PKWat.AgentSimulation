namespace PKWat.AgentSimulation.Core.Crash;

public record SimulationCrashResult(bool IsCrash, string CrashReason)
{
    public static SimulationCrashResult NoCrash => new SimulationCrashResult(false, string.Empty);
    public static SimulationCrashResult Crash(string reason) => new SimulationCrashResult(true, reason);
}

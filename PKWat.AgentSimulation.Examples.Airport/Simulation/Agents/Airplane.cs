namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Math.Extensions;

public class Airplane : SimulationAgent<AirportEnvironment, AirplaneState>
{
    protected override AirplaneState GetInitialState(AirportEnvironment environment)
    {
        return new AirplaneState();
    }

    protected override AirplaneState GetNextState(AirportEnvironment environment, SimulationTime simulationTime)
    {
        if (!State.AskedForLand)
        {
            return State with { AskedForLand = true, AskingForLand = true };
        }

        if (State.AskingForLand && environment.AllowedForLand == Id)
        {
            return State with { AskingForLand = false, LandingStart = simulationTime.Time, LandingFinish = simulationTime.Time + (simulationTime.Step * 10) };
        }

        return State;
    }
}

public record AirplaneState(
    bool AskedForLand = false,
    bool AskingForLand = false,
    TimeSpan? LandingStart = null,
    TimeSpan? LandingFinish = null)
{
    public bool IsBeforeLanding(TimeSpan now) => LandingStart == null || LandingStart > now;
    public bool IsLanding(TimeSpan now) => LandingStart <= now && now <= LandingFinish;
    public double LandingProgress(TimeSpan now) => LandingStart.HasValue && LandingFinish.HasValue ? now.GetProgressBetween(LandingStart.Value, LandingFinish.Value) : 0;
}

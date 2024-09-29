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

        if (State.AskingForLand)
        {
            var givenLine = environment.GetAssignedLine(Id);
            if(givenLine == null)
            {
                return State;
            }

            return State with { AskingForLand = false, LandingLine = givenLine, LandingStart = simulationTime.Time, LandingFinish = simulationTime.Time + (simulationTime.Step * 10) };
        }

        if(State.WaitsForDeparture(simulationTime.Time) && environment.NoPassengerInAirplane(Id))
        {
            return State with { DepartureStart = simulationTime.Time, DepartureFinished = simulationTime.Time + (simulationTime.Step * 10) };
        }

        return State;
    }

    override public bool ShouldBeRemovedFromSimulation(ISimulationContext<AirportEnvironment> simulationContext)
    {
        return State.HasDeparted(simulationContext.SimulationTime.Time);
    }
}

public record AirplaneState(
    bool AskedForLand = false,
    bool AskingForLand = false,
    int? LandingLine = null,
    TimeSpan? LandingStart = null,
    TimeSpan? LandingFinish = null,
    TimeSpan? DepartureStart = null,
    TimeSpan? DepartureFinished = null)
{
    public bool IsBeforeLanding(TimeSpan now) => LandingStart == null || LandingStart > now;
    public bool IsLanding(TimeSpan now) => LandingStart <= now && now <= LandingFinish;
    public double LandingProgress(TimeSpan now) => LandingStart.HasValue && LandingFinish.HasValue ? now.GetProgressBetween(LandingStart.Value, LandingFinish.Value) : 0;
    public bool HasLanded(TimeSpan now) => LandingProgress(now) >= 1.0;
    public bool IsDeparting(TimeSpan now) => DepartureStart <= now && now <= DepartureFinished;
    public double DepartureProgress(TimeSpan now) => DepartureStart.HasValue && DepartureFinished.HasValue ? now.GetProgressBetween(DepartureStart.Value, DepartureFinished.Value) : 0;
    public bool HasDeparted(TimeSpan now) => DepartureProgress(now) >= 1.0;
    public bool WaitsForDeparture(TimeSpan now) => DepartureStart == null && HasLanded(now);
}

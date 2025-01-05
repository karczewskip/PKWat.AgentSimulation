namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Time;
using PKWat.AgentSimulation.Math.Extensions;
using System;

public class Passenger : SimulationAgent<AirportEnvironment, PassengerState>
{
    protected override PassengerState GetInitialState(AirportEnvironment environment)
    {
        return new PassengerState();
    }

    public void SetAirplane(AgentId airplaneId)
    {
        SetState(State with { AirplaneId = airplaneId });
    }

    protected override PassengerState GetNextState(AirportEnvironment environment, IReadOnlySimulationTime simulationTime)
    {
        if (State.AirplaneId == null)
        {
            return State;
        }

        if (environment.AirplaneLanded(State.AirplaneId) && !State.ReadyForCheckout)
        {
            return State with { ReadyForCheckout = true };
        }

        if(State.ReadyForCheckout && State.CheckoutStarted == null && environment.PassengerAllowedToCheckout(Id, State.AirplaneId))
        {
            return State with { CheckoutStarted = simulationTime.Time, CheckoutEnd = simulationTime.Time + simulationTime.Step*3 };
        }

        return State;
    }

    public override bool ShouldBeRemovedFromSimulation(IReadOnlySimulationTime simulationTime)
    {
        return State.Checkouted(simulationTime.Time);
    }
}

public record PassengerState(AgentId? AirplaneId = null, bool ReadyForCheckout = false, TimeSpan? CheckoutStarted = null, TimeSpan? CheckoutEnd = null)
{
    public bool IsCheckouting => CheckoutStarted.HasValue;
    public double CheckoutProgress(TimeSpan now) => now.GetProgressBetween(CheckoutStarted.Value, CheckoutEnd.Value);
    public bool Checkouted(TimeSpan now) => CheckoutEnd.HasValue && CheckoutEnd.Value <= now;
}

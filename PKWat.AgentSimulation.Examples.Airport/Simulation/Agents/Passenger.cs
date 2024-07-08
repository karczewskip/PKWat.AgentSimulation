namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core;
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

    protected override PassengerState GetNextState(AirportEnvironment environment, SimulationTime simulationTime)
    {
        if (State.AirplaneId == null || State.CheckoutStart.HasValue)
        {
            return State;
        }

        if (environment.AirplaneLanded(State.AirplaneId))
        {
            return State with { CheckoutStart = simulationTime.Time, CheckoutEnd = simulationTime.Time + (simulationTime.Step * 10) };
        }

        return State;
    }
}

public record PassengerState(AgentId? AirplaneId = null, TimeSpan? CheckoutStart = null, TimeSpan? CheckoutEnd = null);

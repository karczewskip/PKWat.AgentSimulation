namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;

using PKWat.AgentSimulation.Core;

public class Airplane : SimulationAgent<AirportEnvironment, AirplaneState>
{
    protected override AirplaneState GetInitialState(ISimulationContext<AirportEnvironment> simulationContext)
    {
        return new AirplaneState();
    }

    protected override AirplaneState GetNextState(ISimulationContext<AirportEnvironment> simulationContext)
    {
        return State;
    }
}

public class AirplaneState
{
}

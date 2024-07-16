namespace PKWat.AgentSimulation.Math;

using System;

public class AgentSimulationMathException : Exception
{
    public AgentSimulationMathException(string message) : base(message)
    {
    }

    public AgentSimulationMathException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public AgentSimulationMathException()
    {
    }
}

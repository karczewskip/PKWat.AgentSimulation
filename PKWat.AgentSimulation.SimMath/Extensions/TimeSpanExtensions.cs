namespace PKWat.AgentSimulation.SimMath.Extensions;

using System;

public static class TimeSpanExtensions
{
    public static double GetProgressBetween(this TimeSpan time, TimeSpan start, TimeSpan end)
    {
        if (start > end)
        {
            throw new AgentSimulationMathException("Start time cannot be greater than end time.");
        }

        if (time < start)
        {
            return 0.0;
        }

        if (time > end)
        {
            return 1.0;
        }

        return (time - start).TotalMilliseconds / (end - start).TotalMilliseconds;
    }
}

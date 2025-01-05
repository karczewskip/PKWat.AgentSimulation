using PKWat.AgentSimulation.Core.Environment;

namespace PKWat.AgentSimulation.Examples.LiquidWpf.Simulation
{
    public record BinEnvironmentState(double BinWidth, double BinHeight, DropAcceleration Gravity);

    public class BinEnvironment : DefaultSimulationEnvironment<BinEnvironmentState>
    {
        public double BinWidth => GetState().BinWidth;
        public double BinHeight => GetState().BinHeight;
        public DropAcceleration Gravity => GetState().Gravity;
    }

}

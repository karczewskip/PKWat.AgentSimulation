using PKWat.AgentSimulation.Core;

namespace PKWat.AgentSimulation.Examples.LiquidWpf.Simulation
{
    public class BinEnvironment : DefaultSimulationEnvironment
    {
        public double BinWidth { get; }
        public double BinHeight { get; }

        public DropAcceleration Gravity { get; } = new DropAcceleration(0, 9.8);

        public BinEnvironment(double binWidth, double binHeight)
        {
            BinWidth = binWidth;
            BinHeight = binHeight;
        }
    }

}

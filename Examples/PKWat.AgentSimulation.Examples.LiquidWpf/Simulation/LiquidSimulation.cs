namespace PKWat.AgentSimulation.Examples.LiquidWpf.Simulation
{
    using PKWat.AgentSimulation.Core;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    public class LiquidSimulation(ISimulationBuilder simulationBuilder, LiquidRenderer liquidRenderer)
    {
        public ISimulation CreateSimulation(Action<BitmapSource> render)
        {
            liquidRenderer.Initialize(800, 600);

            return simulationBuilder
                    .CreateNewSimulation<BinEnvironment, BinEnvironmentState>(new BinEnvironmentState(1000, 1000, new DropAcceleration(0, 9.8)))
                    .AddAgents<Drop>(300)
                    .AddCallback(c => RenderAsync(c, render))
                    .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(1000))
                    .Build();
        }

        private async Task RenderAsync(ISimulationContext<BinEnvironment> simulationContext, Action<BitmapSource> render)
        {
            render(liquidRenderer.Draw(simulationContext));
        }
    }
}

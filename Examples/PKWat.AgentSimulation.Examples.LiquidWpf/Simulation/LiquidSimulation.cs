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
                    .CreateNewSimulation(new BinEnvironment(1000, 1000))
                    .AddAgents<Drop>(30)
                    .AddEnvironmentUpdates(UpdateHeatmap)
                    .AddCallback(c => RenderAsync(c, render))
                    .SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(1000))
                    .Build();
        }

        private async Task RenderAsync(ISimulationContext<BinEnvironment> simulationContext, Action<BitmapSource> render)
        {
            render(liquidRenderer.Draw(simulationContext));
        }

        private async Task UpdateHeatmap(ISimulationContext<BinEnvironment> simulationContext)
        {
            var environment = simulationContext.SimulationEnvironment;
            var drops = simulationContext.GetAgents<Drop>().ToArray();
            environment.UpdateHeatmap(drops);
        }
    }
}

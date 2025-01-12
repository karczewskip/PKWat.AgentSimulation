namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations;

using PKWat.AgentSimulation.Core;
using System;
using System.Windows.Media.Imaging;

public interface IExampleSimulationBuilder
{
    ISimulation Build(Action<BitmapSource> drawing);
}

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.Airport;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.AntColony;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.ButterflyEffect;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.GameOfLife;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator;
using System;
using System.Windows.Media.Imaging;

public interface IExampleSimulationBuilder
{
    ISimulation Build(Action<BitmapSource> drawing);
}

public interface IVisualizationDrawer
{
    BitmapSource Draw(ISimulationContext context);
}

public class SimulationsBuilder(
    PreyVsPredatorSimulationBuilder preyVsPredatorSimulationBuilder,
    GameOfLifeSimulationBuilder gameOfLifeSimulationBuilder,
    ButterflyEffectSimulationBuilder butterflyEffectSimulationBuilder,
    AntColonySimulationBuilder antColonySimulationBuilder,
    AirportSimulationBuilder airportSimulationBuilder)
{
    public ISimulation BuildPreyVsPredatorSimulation(Action<BitmapSource> drawing)
    {
        return preyVsPredatorSimulationBuilder.Build(drawing);
    }

    public ISimulation BuildGameOfLifeSimulation(Action<BitmapSource> drawing)
    {
        return gameOfLifeSimulationBuilder.Build(drawing);
    }

    public ISimulation BuildButterflyEffectSimulation(Action<BitmapSource> drawing)
    {
        return butterflyEffectSimulationBuilder.Build(drawing);
    }

    public ISimulation BuildAntColonySimulation(Action<BitmapSource> drawing)
    {
        return antColonySimulationBuilder.Build(drawing);
    }

    public ISimulation BuildAirportSimulation(Action<BitmapSource> drawing)
    {
        return airportSimulationBuilder.Build(drawing);
    }
}

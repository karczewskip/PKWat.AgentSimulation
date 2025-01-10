namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;
public class FoodSource
{
    private readonly double size;
    private readonly ColonyCoordinates coordinates;

    public int X => coordinates.X;
    public int Y => coordinates.Y;
    public double Size => size;

    public FoodSource(double size, ColonyCoordinates coordinates)
    {
        this.size = size;
        this.coordinates = coordinates;
    }
}

namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm;

public class FoodSource
{
    public int Size { get; } = 5;
    public ColonyCoordinates Coordinates { get; }
    public FoodSource(ColonyCoordinates coordinates)
    {
        Coordinates = coordinates;
    }
}

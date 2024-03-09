namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm
{
    public class AntHill
    {
        public int Size { get; } = 5;
        public ColonyCoordinates Coordinates { get; }

        public AntHill(ColonyCoordinates coordinates)
        {
            Coordinates = coordinates;
        }
    }
}

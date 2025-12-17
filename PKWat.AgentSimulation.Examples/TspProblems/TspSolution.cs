namespace PKWat.AgentSimulation.Examples.TspProblems;

public class TspSolution
{
    public List<int> Route { get; init; }
    public double TotalDistance { get; init; }

    private TspSolution() 
    { 
        Route = new List<int>();
    }

    public static TspSolution Create(List<int> route, double totalDistance)
    {
        return new TspSolution
        {
            Route = new List<int>(route),
            TotalDistance = totalDistance
        };
    }

    public static TspSolution Empty()
    {
        return new TspSolution
        {
            Route = new List<int>(),
            TotalDistance = double.MaxValue
        };
    }
}

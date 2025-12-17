namespace PKWat.AgentSimulation.Examples.TspProblems;

public class TspPoint
{
    public double X { get; init; }
    public double Y { get; init; }
    public int Id { get; init; }

    private TspPoint() { }

    public static TspPoint Create(int id, double x, double y)
    {
        return new TspPoint
        {
            Id = id,
            X = x,
            Y = y
        };
    }

    public double DistanceTo(TspPoint other)
    {
        double dx = X - other.X;
        double dy = Y - other.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}

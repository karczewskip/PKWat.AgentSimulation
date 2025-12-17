namespace PKWat.AgentSimulation.Examples.SearchingProblems;

public class SearchPoint
{
    public double X { get; private set; }
    public double Y { get; private set; }
    public double Value { get; private set; }

    private SearchPoint(double x, double y, double value)
    {
        X = x;
        Y = y;
        Value = value;
    }

    public static SearchPoint Create(double x, double y, double value)
    {
        return new SearchPoint(x, y, value);
    }

    public double DistanceTo(SearchPoint other)
    {
        return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
    }

    public void UpdateValue(double newValue)
    {
        Value = newValue;
    }
}

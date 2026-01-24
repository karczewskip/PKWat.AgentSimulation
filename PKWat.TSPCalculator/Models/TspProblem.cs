namespace PKWat.TSPCalculator.Models;

public class TspProblem
{
    public string Name { get; set; } = string.Empty;
    public List<Point> Points { get; set; } = new();
}

public class Point
{
    public int Id { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}

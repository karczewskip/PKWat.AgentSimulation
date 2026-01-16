namespace PKWat.AgentSimulation.Examples.SearchingProblems.Agents;

using PKWat.AgentSimulation.Core.Agent;

public class SearchAgent : SimpleSimulationAgent
{
    public double CurrentX { get; private set; }
    public double CurrentY { get; private set; }
    public double CurrentValue { get; private set; }
    public double BestValue { get; private set; }
    public SearchPoint? BestPoint { get; private set; }

    public void SetPosition(double x, double y, double value)
    {
        CurrentX = x;
        CurrentY = y;
        CurrentValue = value;

        if (BestPoint == null || value > BestValue)
        {
            BestValue = value;
            BestPoint = SearchPoint.Create(x, y, value);
        }
    }

    public void UpdateBestIfBetter(SearchPoint point)
    {
        if (point.Value > BestValue)
        {
            BestValue = point.Value;
            BestPoint = point;
        }
    }
}

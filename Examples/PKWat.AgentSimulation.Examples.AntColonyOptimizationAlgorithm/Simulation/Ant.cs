namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm.Simulation;

using PKWat.AgentSimulation.Core.Agent;

public class Ant : SimpleSimulationAgent
{
    public ColonyCoordinates Coordinates { get; private set; } = ColonyCoordinates.CreateAtOrigin();
    public bool IsCarryingFood { get; private set; } = false;
    public bool IsAfterHillVisit { get; private set; } = false;
    public ColonyDirection Direction { get; set; } = ColonyDirection.None;
    public int PathLength { get; set; } = 1;

    public void GetFood()
    {
        IsCarryingFood = true;
        IsAfterHillVisit = false;
        PathLength = 1;
        ChangeToOpositeDirection();
    }

    public void VisitHill()
    {
        IsCarryingFood = false;
        IsAfterHillVisit = true;
        PathLength = 1;
        ChangeToOpositeDirection();
    }

    private void ChangeToOpositeDirection()
    {
        Direction = Direction switch
        {
            ColonyDirection.Up => ColonyDirection.Down,
            ColonyDirection.UpRight => ColonyDirection.DownLeft,
            ColonyDirection.Right => ColonyDirection.Left,
            ColonyDirection.DownRight => ColonyDirection.UpLeft,
            ColonyDirection.Down => ColonyDirection.Up,
            ColonyDirection.DownLeft => ColonyDirection.UpRight,
            ColonyDirection.Left => ColonyDirection.Right,
            ColonyDirection.UpLeft => ColonyDirection.DownRight,
            _ => ColonyDirection.None
        };
    }
}

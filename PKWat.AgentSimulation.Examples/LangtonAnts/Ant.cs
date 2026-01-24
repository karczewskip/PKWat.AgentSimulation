namespace PKWat.AgentSimulation.Examples.LangtonAnts;

public class Ant
{
    public int X { get; set; }
    public int Y { get; set; }
    public AntDirection Direction { get; set; }
    public AntType Type { get; set; }
    public AntPair Pair { get; set; }
    
    public int PreviousX { get; set; }
    public int PreviousY { get; set; }

    public Ant(int x, int y, AntDirection direction, AntType type, AntPair pair)
    {
        X = x;
        Y = y;
        Direction = direction;
        Type = type;
        Pair = pair;
        PreviousX = x;
        PreviousY = y;
    }

    public void TurnLeft()
    {
        Direction = (AntDirection)(((int)Direction + 3) % 4);
    }

    public void TurnRight()
    {
        Direction = (AntDirection)(((int)Direction + 1) % 4);
    }

    public (int x, int y) GetForwardPosition()
    {
        return Direction switch
        {
            AntDirection.North => (X, Y - 1),
            AntDirection.East => (X + 1, Y),
            AntDirection.South => (X, Y + 1),
            AntDirection.West => (X - 1, Y),
            _ => throw new InvalidOperationException("Invalid direction")
        };
    }

    public void SaveCurrentPosition()
    {
        PreviousX = X;
        PreviousY = Y;
    }
}

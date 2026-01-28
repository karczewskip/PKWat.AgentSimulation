using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.RandomNumbers;
using System.ComponentModel;
using System.Drawing;

namespace PKWat.AgentSimulation.Examples.LangtonAnts;

public class Ant(IRandomNumbersGenerator randomNumbersGenerator) : SimpleSimulationAgent
{
    public int X { get; set; }
    public int Y { get; set; }
    public AntDirection Direction { get; set; }
    public AntType Type { get; set; }
    public AntPair Pair { get; set; }

    public void Initialize(int x, int y, AntDirection direction, AntType type, AntPair pair)
    {
        X = x;
        Y = y;
        Direction = direction;
        Type = type;
        Pair = pair;
    }

    public AntDirection CalculateLeft()
    {
        return (AntDirection)(((int)Direction + 3) % 4);
    }

    public AntDirection CalculateRight()
    {
        return (AntDirection)(((int)Direction + 1) % 4);
    }

    public (int x, int y) CalculateForwardMove(AntDirection direction)
    {
        return direction switch
        {
            AntDirection.North => (X, Y - 1),
            AntDirection.East => (X + 1, Y),
            AntDirection.South => (X, Y + 1),
            AntDirection.West => (X - 1, Y),
            _ => throw new InvalidOperationException("Invalid direction")
        };
    }

    public void SetTargetPosition(int x, int y, AntDirection antDirection)
    {
        X = x;
        Y = y;
        Direction = antDirection;
    }

    internal (int antX, int antY) GetCurrentPosition()
    {
        return (X, Y);
    }

    internal (Color color, int newX, int newY, AntDirection antDirection) CalculateNewPositionAndColor(Color currentColor)
    {
        Color newCellColor;
        AntDirection newDirection;
        (int x, int y) forwardMove;

        // Determine new direction based on current color and ant type
        if (Pair.IsMyColor(currentColor))
        {
            // A. Color belongs to ant's pair
            if (Type == AntType.A)
            {
                // Ant A: Color1 -> RIGHT, Color2 -> LEFT
                if (currentColor.ToArgb() == Pair.Color1.ToArgb())
                {
                    newDirection = CalculateRight();
                }
                else
                {
                    newDirection = CalculateLeft();
                }
            }
            else
            {
                // Ant B: Color1 -> LEFT, Color2 -> RIGHT
                if (currentColor.ToArgb() == Pair.Color1.ToArgb())
                {
                    newDirection = CalculateLeft();
                }
                else
                {
                    newDirection = CalculateRight();
                }
            }

            newCellColor = Pair.GetOppositeColor(currentColor);

            forwardMove = CalculateForwardMove(newDirection);
            return (newCellColor, forwardMove.x, forwardMove.y, newDirection);
        }

        if (randomNumbersGenerator.Next(2) == 0)
        {
            newDirection = CalculateLeft();
            if (Type == AntType.A)
            {
                newCellColor = Pair.Color1;
            }
            else
            {
                newCellColor = Pair.Color2;
            }
        }
        else
        {
            newDirection = CalculateRight();
            if (Type == AntType.A)
            {
                newCellColor = Pair.Color2;
            }
            else
            {
                newCellColor = Pair.Color1;
            }
        }

        forwardMove = CalculateForwardMove(newDirection);
        return (newCellColor, forwardMove.x, forwardMove.y, newDirection);
    }
}

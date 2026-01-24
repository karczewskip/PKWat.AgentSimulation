namespace PKWat.AgentSimulation.Examples.LangtonAnts.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using System.Drawing;
using System.Threading.Tasks;

public class CalculateAntMovement : ISimulationStage
{
    private readonly IRandomNumbersGenerator _randomGenerator;

    public CalculateAntMovement(IRandomNumbersGenerator randomGenerator)
    {
        _randomGenerator = randomGenerator;
    }

    private class AntMove
    {
        public Ant Ant { get; set; }
        public int TargetX { get; set; }
        public int TargetY { get; set; }
        public Color NewCellColor { get; set; }
        public bool IsValid { get; set; }
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<LangtonAntsEnvironment>();

        // Save all current positions before processing
        foreach (var ant in environment.AllAnts)
        {
            ant.SaveCurrentPosition();
        }

        // Calculate proposed moves for all ants
        var proposedMoves = new List<AntMove>();

        foreach (var ant in environment.AllAnts)
        {
            var currentColor = environment.GetColorAt(ant.X, ant.Y);
            var move = new AntMove { Ant = ant, IsValid = true };

            // Check if color belongs to ant's pair
            if (ant.Pair.IsMyColor(currentColor))
            {
                // A. Color belongs to ant's pair
                // Determine rotation based on current color and ant type
                if (ant.Type == AntType.A)
                {
                    // Ant A: Color1 -> RIGHT, Color2 -> LEFT
                    if (currentColor.ToArgb() == ant.Pair.Color1.ToArgb())
                    {
                        ant.TurnRight();
                    }
                    else
                    {
                        ant.TurnLeft();
                    }
                }
                else // AntType.B
                {
                    // Ant B: Color1 -> LEFT, Color2 -> RIGHT
                    if (currentColor.ToArgb() == ant.Pair.Color1.ToArgb())
                    {
                        ant.TurnLeft();
                    }
                    else
                    {
                        ant.TurnRight();
                    }
                }

                // The new cell color is the opposite color in the pair
                move.NewCellColor = ant.Pair.GetOppositeColor(currentColor);
            }
            else
            {
                // B. Foreign color (belongs to another pair)
                // Randomly select one of the ant's own two colors
                var rolledColor = _randomGenerator.GetNextBool() ? ant.Pair.Color1 : ant.Pair.Color2;

                // Use rolledColor to determine rotation
                if (ant.Type == AntType.A)
                {
                    if (rolledColor.ToArgb() == ant.Pair.Color1.ToArgb())
                    {
                        ant.TurnRight();
                    }
                    else
                    {
                        ant.TurnLeft();
                    }
                }
                else // AntType.B
                {
                    if (rolledColor.ToArgb() == ant.Pair.Color1.ToArgb())
                    {
                        ant.TurnLeft();
                    }
                    else
                    {
                        ant.TurnRight();
                    }
                }

                // The new cell color is rolledColor
                move.NewCellColor = rolledColor;
            }

            // Calculate target position
            var (targetX, targetY) = ant.GetForwardPosition();
            var (wrappedX, wrappedY) = environment.WrapCoordinates(targetX, targetY);
            move.TargetX = wrappedX;
            move.TargetY = wrappedY;

            proposedMoves.Add(move);
        }

        // Validate moves - check for collisions with previous positions
        foreach (var move in proposedMoves)
        {
            if (environment.WasOccupiedInPreviousEpoch(move.TargetX, move.TargetY))
            {
                move.IsValid = false;
            }
        }

        // Execute valid moves
        foreach (var move in proposedMoves)
        {
            if (move.IsValid)
            {
                // Change current cell color
                environment.SetColorAt(move.Ant.X, move.Ant.Y, move.NewCellColor);
                
                // Move ant to target position
                move.Ant.X = move.TargetX;
                move.Ant.Y = move.TargetY;
            }
            // If move is invalid, ant stays in place but cell color still changes
            else
            {
                // Change current cell color even though ant doesn't move
                environment.SetColorAt(move.Ant.X, move.Ant.Y, move.NewCellColor);
            }
        }
    }
}

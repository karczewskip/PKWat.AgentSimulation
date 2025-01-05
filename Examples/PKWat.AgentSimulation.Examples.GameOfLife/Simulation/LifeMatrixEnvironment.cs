using PKWat.AgentSimulation.Core;

namespace PKWat.AgentSimulation.Examples.GameOfLife.Simulation;

public record LifeMatrixCellState(AgentId AgentId, bool IsAlive);

public record LifeMatrixEnvironmentState(Dictionary<AgentId, (int X, int Y)> AgentsCoordinates, bool[,] Matrix, int Generation);

public class LifeMatrixEnvironment : DefaultSimulationEnvironment<LifeMatrixEnvironmentState>
{
    public void PlaceAgents(IReadOnlyCollection<AgentId> Agents)
    {
        var agentsCoordinates = new Dictionary<AgentId, (int X, int Y)>();
        for (int i = 0; i < Agents.Count; i++)
        {
            var x = i / GetWidth();
            var y = i % GetWidth();
            agentsCoordinates[Agents.ElementAt(i)] = (x, y);
        }

        var state = GetState();

        LoadState(new LifeMatrixEnvironmentState(agentsCoordinates, state.Matrix, state.Generation));
    }

    public void SetNewMatrix(IReadOnlyCollection<LifeMatrixCellState> lifeMatrixCellStates)
    {
        var state = GetState();
        var newMatrix = new bool[GetWidth(), GetHeight()];
        foreach (var cellState in lifeMatrixCellStates)
        {
            var coordinates = state.AgentsCoordinates[cellState.AgentId];
            newMatrix[coordinates.X, coordinates.Y] = cellState.IsAlive;
        }
        LoadState(new LifeMatrixEnvironmentState(state.AgentsCoordinates, newMatrix, state.Generation + 1));
    }

    public int GetAliveNeighboursCount(AgentId id)
    {
        var state = GetState();
        var coordinates = state.AgentsCoordinates[id];
        var aliveNeighbours = 0;
        var width = GetWidth();
        var height = GetHeight();
        for (int i = coordinates.X - 1; i <= coordinates.X + 1; i++)
        {
            for (int j = coordinates.Y - 1; j <= coordinates.Y + 1; j++)
            {
                if (i == coordinates.X && j == coordinates.Y)
                {
                    continue;
                }
                if (i >= 0 && i < width && j >= 0 && j < height && state.Matrix[i, j])
                {
                    aliveNeighbours++;
                }
            }
        }
        return aliveNeighbours;
    }

    public int GetWidth()
    {
        var state = GetState();
        return state.Matrix.GetLength(0);
    }

    public int GetHeight()
    {
        var state = GetState();
        return state.Matrix.GetLength(1);
    }

    public bool IsCellAlive(int x, int y)
    {
        var state = GetState();
        return state.Matrix[x, y];
    }

    public override object CreateSnapshot()
    {
        return new { };
    }
}

using PKWat.AgentSimulation.Core;

namespace PKWat.AgentSimulation.Examples.GameOfLife.GPU.Simulation;

public record LifeMatrixEnvironmentState(bool[,] Matrix, int Generation);

public class LifeMatrixEnvironment(
    IRandomNumbersGenerator randomNumbersGenerator, 
    NewMatrixOnGPUGenerator newMatrixOnGPUGenerator) : DefaultSimulationEnvironment<LifeMatrixEnvironmentState>
{
    public void Initialize(int width, int height)
    {
        var matrix = new bool[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                matrix[i, j] = randomNumbersGenerator.GetNextBool();
            }
        }
        LoadState(new LifeMatrixEnvironmentState(matrix, 0));
    }

    public void Update()
    {
        var state = GetState();

        var width = GetWidth();
        var height = GetHeight();

        var newMatrix = new bool[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var aliveNeighbours = GetAliveNeighboursCount(i, j);
                if (state.Matrix[i, j])
                {
                    var isStillAlive = aliveNeighbours == 2 || aliveNeighbours == 3;
                    newMatrix[i, j] = isStillAlive;
                }
                else
                {
                    var isBorn = aliveNeighbours == 3;
                    newMatrix[i, j] = isBorn;
                }
            }
        }

        LoadState(new LifeMatrixEnvironmentState(newMatrix, state.Generation + 1));
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

    private int GetAliveNeighboursCount(int x, int y)
    {
        var state = GetState();

        var width = GetWidth();
        var height = GetHeight();

        var aliveNeighbours = 0;

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i == x && j == y)
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

    public void UpdateOnGPU()
    {
        var newMatrix = newMatrixOnGPUGenerator.Generate(GetState().Matrix);

        LoadState(new LifeMatrixEnvironmentState(newMatrix, GetState().Generation + 1));
    }
}

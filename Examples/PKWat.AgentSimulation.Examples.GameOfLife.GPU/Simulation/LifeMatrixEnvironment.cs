using PKWat.AgentSimulation.Core;

namespace PKWat.AgentSimulation.Examples.GameOfLife.GPU.Simulation;

public record LifeMatrixEnvironmentState(bool[,] Matrix, int Generation);

public class LifeMatrixEnvironment(
    IRandomNumbersGenerator randomNumbersGenerator, 
    NewMatrixOnGPUGenerator newMatrixOnGPUGenerator) : DefaultSimulationEnvironment<LifeMatrixEnvironmentState>
{
    private bool[][,] matrices;
    private int currentMatrixIndex = 0;
    private int width;
    private int height;

    public void Initialize(int width, int height)
    {
        this.width = width;
        this.height = height;

        matrices = new bool[2][,];
        matrices[0] = new bool[width, height];
        matrices[1] = new bool[width, height];
        var matrix = matrices[currentMatrixIndex];
        currentMatrixIndex = (currentMatrixIndex + 1) % 2;

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

        var newMatrix = matrices[currentMatrixIndex];
        currentMatrixIndex = (currentMatrixIndex + 1) % 2;

        var numberOfThreads = 8;

        Parallel.For(0, numberOfThreads, i =>
        {
            var start = i * width / numberOfThreads;
            var end = (i + 1) * width / numberOfThreads;
            for (int x = start; x < end; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var aliveNeighbours = GetAliveNeighboursCount(x, y, width, height);
                    if (state.Matrix[x, y])
                    {
                        var isStillAlive = aliveNeighbours == 2 || aliveNeighbours == 3;
                        newMatrix[x, y] = isStillAlive;
                    }
                    else
                    {
                        var isBorn = aliveNeighbours == 3;
                        newMatrix[x, y] = isBorn;
                    }
                }
            }
        });

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

    private int GetAliveNeighboursCount(int x, int y, int width, int height)
    {
        var state = GetState();

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

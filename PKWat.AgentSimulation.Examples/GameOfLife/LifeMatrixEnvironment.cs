using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.RandomNumbers;

namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.GameOfLife;

public class LifeMatrixEnvironment(
    IRandomNumbersGenerator randomNumbersGenerator) : DefaultSimulationEnvironment
{
    public bool[,] Matrix => matrices[currentMatrixIndex];

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
    }

    public void Update(int numberOfThreads)
    {
        var newMatrix = matrices[currentMatrixIndex];
        currentMatrixIndex = (currentMatrixIndex + 1) % 2;

        Parallel.For(0, numberOfThreads, new ParallelOptions() { MaxDegreeOfParallelism = numberOfThreads }, i =>
        {
            var start = i * width / numberOfThreads;
            var end = (i + 1) * width / numberOfThreads;
            for (int x = start; x < end; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var aliveNeighbours = GetAliveNeighboursCount(x, y, width, height);
                    if (Matrix[x, y])
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
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public bool IsCellAlive(int x, int y)
    {
        return Matrix[x, y];
    }

    private int GetAliveNeighboursCount(int x, int y, int width, int height)
    {
        var aliveNeighbours = 0;

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i == x && j == y)
                {
                    continue;
                }
                if (i >= 0 && i < width && j >= 0 && j < height && Matrix[i, j])
                {
                    aliveNeighbours++;
                }
            }
        }

        return aliveNeighbours;
    }
}

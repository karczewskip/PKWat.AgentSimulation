using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;

namespace PKWat.AgentSimulation.Examples.GameOfLife.GPU.Simulation
{
    public class NewMatrixOnGPUGenerator
    {
        public bool[,] Generate(bool[,] previousMatrix)
        {
            var width = previousMatrix.GetLength(0);
            var height = previousMatrix.GetLength(1);

            var data1d = new bool[width * height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    data1d[i * height + j] = previousMatrix[i, j];
                }
            }

            // Konfiguracja CUDAfy
            CudafyModes.Target = eGPUType.Cuda;
            CudafyModes.Compiler = eGPUCompiler.CudaNvcc;
            var gpu = CudafyHost.GetDevice(eGPUType.Cuda);

            // Tłumaczenie kernela i ładowanie modułu
            var module = CudafyTranslator.Cudafy();
            gpu.LoadModule(module);

            var gpuData = gpu.Allocate(data1d);
            var gpuOuptutDate = gpu.Allocate(data1d);

            gpu.CopyToDevice(data1d, gpuData);

            gpu.Launch(width, height).UpdateMatrix(gpuData, gpuOuptutDate, width, height);

            gpu.CopyFromDevice(gpuOuptutDate, data1d);

            gpu.Free(gpuData);
            gpu.Free(gpuOuptutDate);

            var matrix = new bool[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    matrix[i, j] = data1d[i * height + j];
                }
            }

            return matrix;
        }

        [Cudafy]
        public static void UpdateMatrix(GThread thread, bool[] data, bool[] outputDate, int width, int height)
        {
            var x = thread.blockIdx.x;
            var y = thread.blockIdx.y;

            var aliveNeighbours = 0;

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y)
                    {
                        continue;
                    }
                    if (i >= 0 && i < width && j >= 0 && j < height && data[y*width + x])
                    {
                        aliveNeighbours++;
                    }
                }
            }

            if (data[y * width + x])
            {
                var isStillAlive = aliveNeighbours == 2 || aliveNeighbours == 3;
                outputDate[y * width + x] = isStillAlive;
            }
            else
            {
                var isBorn = aliveNeighbours == 3;
                outputDate[y * width + x] = isBorn;
            }
        }
    }
}

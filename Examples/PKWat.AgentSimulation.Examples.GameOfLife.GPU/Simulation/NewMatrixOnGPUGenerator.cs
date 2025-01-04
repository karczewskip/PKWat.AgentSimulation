using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;

namespace PKWat.AgentSimulation.Examples.GameOfLife.GPU.Simulation
{
    public class NewMatrixOnGPUGenerator
    {
        private Context _context;
        private Accelerator _device;

        public void Initialize()
        {
            _context = Context.Create(builder => builder.Cuda().CPU().EnableAlgorithms());
            _device = _context.GetPreferredDevice(false).CreateAccelerator(_context);
        }

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

            var gpuData = _device.Allocate1D(data1d);
            var gpuOuptutDate = _device.Allocate1D<bool>(width * height);

            //var kernel = _device.LoadAutoGroupedStreamKernel<Index1D, ArrayView<bool>, ArrayView<bool>>(UpdateMatrix);

            //gpu.CopyToDevice(data1d, gpuData);

            //gpu.Launch(width, height).UpdateMatrix(gpuData, gpuOuptutDate, width, height);

            //gpu.CopyFromDevice(gpuOuptutDate, data1d);

            //gpu.Free(gpuData);
            //gpu.Free(gpuOuptutDate);

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

        public static void UpdateMatrix(bool[] data, bool[] outputDate, int width, int height)
        {
            var x = 1; // thread.blockIdx.x;
            var y = 1; // thread.blockIdx.y;

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

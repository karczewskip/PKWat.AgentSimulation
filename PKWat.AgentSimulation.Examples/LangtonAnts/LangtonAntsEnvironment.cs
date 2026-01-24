namespace PKWat.AgentSimulation.Examples.LangtonAnts;

using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.SimMath.Colors;
using System.Drawing;

public class LangtonAntsEnvironment(IRandomNumbersGenerator randomNumbersGenerator, ColorsGenerator colorsGenerator) : DefaultSimulationEnvironment
{
    public Color[,] Grid { get; private set; }
    public List<AntPair> AntPairs { get; private set; } = new List<AntPair>();
    public List<Ant> AllAnts { get; private set; } = new List<Ant>();

    public int Width { get; private set; }
    public int Height { get; private set; }

    public void Initialize(int width, int height, int pairCount)
    {
        Width = width;
        Height = height;
        Grid = new Color[width, height];

        // Generate unique colors for each pair using HSV color space
        var allColors = colorsGenerator.Generate(pairCount * 2);
        
        // Create ant pairs
        AntPairs.Clear();
        AllAnts.Clear();
        
        for (int i = 0; i < pairCount; i++)
        {
            var color1 = allColors[i * 2];
            var color2 = allColors[i * 2 + 1];
            var pair = new AntPair(i, color1, color2);
            AntPairs.Add(pair);
        }

        // Initialize grid with random colors from all pairs
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var randomPairIndex = randomNumbersGenerator.Next(pairCount);
                var randomPair = AntPairs[randomPairIndex];
                var randomColor = randomNumbersGenerator.GetNextBool() ? randomPair.Color1 : randomPair.Color2;
                Grid[x, y] = randomColor;
            }
        }

        // Place ants at random positions
        foreach (var pair in AntPairs)
        {
            // Ant A
            var antAX = randomNumbersGenerator.Next(width);
            var antAY = randomNumbersGenerator.Next(height);
            var antADir = (AntDirection)randomNumbersGenerator.Next(4);
            var antA = new Ant(antAX, antAY, antADir, AntType.A, pair);
            pair.AntA = antA;
            AllAnts.Add(antA);

            // Ant B
            var antBX = randomNumbersGenerator.Next(width);
            var antBY = randomNumbersGenerator.Next(height);
            var antBDir = (AntDirection)randomNumbersGenerator.Next(4);
            var antB = new Ant(antBX, antBY, antBDir, AntType.B, pair);
            pair.AntB = antB;
            AllAnts.Add(antB);
        }
    }
    public (int x, int y) WrapCoordinates(int x, int y)
    {
        var wrappedX = ((x % Width) + Width) % Width;
        var wrappedY = ((y % Height) + Height) % Height;
        return (wrappedX, wrappedY);
    }

    public Color GetColorAt(int x, int y)
    {
        var (wrappedX, wrappedY) = WrapCoordinates(x, y);
        return Grid[wrappedX, wrappedY];
    }

    public void SetColorAt(int x, int y, Color color)
    {
        var (wrappedX, wrappedY) = WrapCoordinates(x, y);
        Grid[wrappedX, wrappedY] = color;
    }

    public bool WasOccupiedInPreviousEpoch(int x, int y)
    {
        var (wrappedX, wrappedY) = WrapCoordinates(x, y);
        return AllAnts.Any(ant => ant.PreviousX == wrappedX && ant.PreviousY == wrappedY);
    }
}

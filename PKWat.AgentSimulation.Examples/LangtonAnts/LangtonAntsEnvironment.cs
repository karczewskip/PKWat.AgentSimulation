namespace PKWat.AgentSimulation.Examples.LangtonAnts;

using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.SimMath.Colors;
using System;
using System.Drawing;

public class LangtonAntsEnvironment(IRandomNumbersGenerator randomNumbersGenerator, ColorsGenerator colorsGenerator) : DefaultSimulationEnvironment
{
    public Color[,] Grid { get; private set; }
    public bool[,] OccupiedGrid { get; private set; }

    public int Width { get; private set; }
    public int Height { get; private set; }

    public void Initialize(int width, int height, Color[] allColors)
    {
        Width = width;
        Height = height;
        Grid = new Color[width, height];
        OccupiedGrid = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var randomColor = allColors[randomNumbersGenerator.Next(allColors.Length)];
                Grid[x, y] = randomColor;
            }
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

    public bool WasFreeInPreviousEpoch(int x, int y)
    {
        return !OccupiedGrid[x, y];
    }

    internal void ClearOccupiedGrid()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                OccupiedGrid[x, y] = false;
            }
        }
    }

    internal void MarkOccupied(int antX, int antY)
    {
        OccupiedGrid[antX, antY] = true;
    }
}

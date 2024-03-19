namespace PKWat.AgentSimulations.Examples.CollisionDetection;

using PKWat.AgentSimulation.Drawing;
using System.Drawing;

public class ColorInitializer
{
    private readonly ColorsGenerator _colorsGenerator;

    public ColorInitializer(ColorsGenerator colorsGenerator)
    {
        _colorsGenerator = colorsGenerator;
    }

    private Color[] _colors;
    private int takenColors = 0;

    public void Initialize(int numberOfColors)
    {
        _colors = _colorsGenerator.Generate(numberOfColors);
    }

    public Color GetNext()
    {
        return Color.Aqua;
        return _colors[(takenColors++)%_colors.Length];
    }
}

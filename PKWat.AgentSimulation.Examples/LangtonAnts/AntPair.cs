namespace PKWat.AgentSimulation.Examples.LangtonAnts;

using System.Drawing;

public class AntPair
{
    public int PairId { get; set; }
    public Color Color1 { get; set; }
    public Color Color2 { get; set; }
    public Ant AntA { get; set; }
    public Ant AntB { get; set; }

    public AntPair(int pairId, Color color1, Color color2)
    {
        PairId = pairId;
        Color1 = color1;
        Color2 = color2;
    }

    public bool IsMyColor(Color color)
    {
        return Color1.ToArgb() == color.ToArgb() || Color2.ToArgb() == color.ToArgb();
    }

    public Color GetOppositeColor(Color color)
    {
        if (Color1.ToArgb() == color.ToArgb())
            return Color2;
        if (Color2.ToArgb() == color.ToArgb())
            return Color1;
        
        throw new InvalidOperationException("Color is not part of this pair");
    }
}

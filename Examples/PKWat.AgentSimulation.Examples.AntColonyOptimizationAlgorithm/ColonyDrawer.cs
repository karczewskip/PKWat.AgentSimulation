namespace PKWat.AgentSimulation.Examples.AntColonyOptimizationAlgorithm
{
    using System.Drawing;
    using System.Windows.Media.Imaging;
    using PKWat.AgentSimulation.Drawing;

    public class ColonyDrawer
    {
        private const int AntSize = 10;

        private Bitmap _bmp;

        public void Initialize(int width, int height)
        {
            _bmp = new Bitmap(width, height);
            _bmp.SetResolution(96, 96);
        }

        public BitmapSource Draw(Ant[] ants)
        {
            using var graphic = Graphics.FromImage(_bmp);

            foreach (Ant ant in ants)
            {
                graphic.FillEllipse(System.Drawing.Brushes.Red, (float)ant.X, (float)ant.Y, AntSize, AntSize);
            }

            return _bmp.ConvertToBitmapSource();
        }
    }
}

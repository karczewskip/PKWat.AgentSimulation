namespace PKWat.AgentSimulation.Math.Extensions
{
    public static class DoubleExtensions
    {
        public static double ScaleToView(this double value, double previousView, double newView)
        {
            return value * newView / previousView;
        }
    }
}

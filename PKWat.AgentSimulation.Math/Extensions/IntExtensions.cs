namespace PKWat.AgentSimulation.Math.Extensions
{
    public static class IntExtensions
    {
        public static double ScaleToView(this int value, double previousView, double newView)
        {
            return value * newView / previousView;
        }
    }
}

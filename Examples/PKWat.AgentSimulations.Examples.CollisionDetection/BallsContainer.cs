namespace PKWat.AgentSimulations.Examples.CollisionDetection
{
    public class BallsContainer
    {
        public double Width { get; }
        public double Height { get; }
        public BallAcceleration Gravity { get; }

        public BallsContainer(double width, double height, BallAcceleration gravity)
        {
            Width = width;
            Height = height;
            Gravity = gravity;
        }
    }
}

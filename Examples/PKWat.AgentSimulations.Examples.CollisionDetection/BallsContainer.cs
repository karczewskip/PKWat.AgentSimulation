namespace PKWat.AgentSimulations.Examples.CollisionDetection
{
    public class BallsContainer
    {
        private Dictionary<Backet, Ball[]> _ballsInBackets = new();

        public double Width { get; }
        public double Height { get; }
        public BallAcceleration Gravity { get; }
        public double BallRadius { get; }
        public double VelocityLossAfterBounce => 0.5;

        public BallsContainer(double width, double height, BallAcceleration gravity, double ballRadius)
        {
            Width = width;
            Height = height;
            Gravity = gravity;
            BallRadius = ballRadius;
        }

        public Ball[] GetNearestBalls(BallCoordinates coordinates)
        {
            return Backet.GetNearestBackets(coordinates, BallRadius).Where(x => _ballsInBackets.ContainsKey(x)).SelectMany(x => _ballsInBackets[x]).ToArray();
        }

        public void UpdateNearestBalls(IEnumerable<Ball> balls)
        {
            _ballsInBackets.Clear();

            var groupedBalls = balls.GroupBy(ball => Backet.GetFromCoordinatesAndRadius(ball.State.Coordinates, BallRadius));

            foreach(var ballsGroup in groupedBalls)
            {
                _ballsInBackets.Add(ballsGroup.Key, [.. ballsGroup]);
            }
        }

        private record Backet(int x, int y)
        {
            public static Backet GetFromCoordinatesAndRadius(BallCoordinates coordinates, double radius) 
            {
                var x = (int)(coordinates.X / radius);
                var y = (int)(coordinates.Y / radius);

                return new Backet(x, y);
            }

            public static Backet[] GetNearestBackets(BallCoordinates coordinates, double radius)
            {
                var centralBacketX = (int)(coordinates.X / radius);
                var centralBacketY = (int)(coordinates.Y / radius);

                return
                [
                    new Backet(centralBacketX-1, centralBacketY-1),
                    new Backet(centralBacketX-1, centralBacketY),
                    new Backet(centralBacketX-1, centralBacketY+1),
                    new Backet(centralBacketX, centralBacketY-1),
                    new Backet(centralBacketX, centralBacketY),
                    new Backet(centralBacketX, centralBacketY+1),
                    new Backet(centralBacketX+1, centralBacketY-1),
                    new Backet(centralBacketX+1, centralBacketY),
                    new Backet(centralBacketX+1, centralBacketY+1),
                ];
            }
        }
    }
}

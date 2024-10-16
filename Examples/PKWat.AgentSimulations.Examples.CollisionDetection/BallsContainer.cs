namespace PKWat.AgentSimulations.Examples.CollisionDetection
{
    using PKWat.AgentSimulation.Core;
    using System.Net.Http.Headers;
    using static PKWat.AgentSimulations.Examples.CollisionDetection.BallsContainerState;

    public record BallsContainerState(
        Dictionary<Backet, Ball[]> BallsInBackets, 
        double Width, 
        double Height, 
        BallAcceleration Gravity, 
        double BallRadius, 
        double VelocityLossAfterBounce);


    public record Backet(int x, int y)
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

    public class BallsContainer : DefaultSimulationEnvironment<BallsContainerState>
    {
        public override object CreateSnapshot()
        {
            var state = GetState();

            return new
            {
                Width = state.Width,
                Height = state.Height,
                Gravity = state.Gravity,
                BallRadius = state.BallRadius,
                BallsInBackets = state.BallsInBackets.Select(x => new { Backet = x.Key, Balls = x.Value }).ToArray()
            };
        }

        public Ball[] GetNearestBalls(BallCoordinates coordinates)
        {
            return Backet.GetNearestBackets(coordinates, GetState().BallRadius).Where(x => GetState().BallsInBackets.ContainsKey(x)).SelectMany(x => GetState().BallsInBackets[x]).ToArray();
        }

        public void UpdateNearestBalls(IEnumerable<Ball> balls)
        {
            GetState().BallsInBackets.Clear();

            var groupedBalls = balls.GroupBy(ball => Backet.GetFromCoordinatesAndRadius(ball.State.Coordinates, GetState().BallRadius));

            foreach(var ballsGroup in groupedBalls)
            {
                GetState().BallsInBackets.Add(ballsGroup.Key, [.. ballsGroup]);
            }
        }

        public double GetWidth() => GetState().Width;

        public double GetHeight() => GetState().Height;

        public BallAcceleration GetGravity() => GetState().Gravity;

        public double GetBallRadius() => GetState().BallRadius;
    }
}

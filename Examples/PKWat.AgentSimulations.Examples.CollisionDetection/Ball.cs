using PKWat.AgentSimulation.Core;
using System.Drawing;

namespace PKWat.AgentSimulations.Examples.CollisionDetection
{
    public record BallState(BallCoordinates Coordinates, BallVelocity Velocity, Color Color);

    public class Ball : SimulationAgent<BallsContainer, BallState>
    {
        private readonly IRandomNumbersGenerator _randomNumbersGenerator;
        private readonly ColorInitializer _colorInitializer;

        public Ball(IRandomNumbersGenerator randomNumbersGenerator, ColorInitializer colorInitializer)
        {
            _randomNumbersGenerator = randomNumbersGenerator;
            _colorInitializer = colorInitializer;
        }

        protected override BallState GetInitialState(BallsContainer environment)
        {
            var x = _randomNumbersGenerator.NextDouble() * environment.Width;
            var y = environment.Height * 0.1 + _randomNumbersGenerator.NextDouble() * environment.Height / 2;

            return new BallState(
                new BallCoordinates(x, y),
                new BallVelocity(50 * (_randomNumbersGenerator.NextDouble() - 0.5), 0),
                _colorInitializer.GetNext());
        }

        protected override BallState GetNextState(BallsContainer environment, SimulationTime simulationTime)
        {
            var timeInSeconds = simulationTime.Step.TotalSeconds;

            var newVelocity = State.Velocity.ApplyAcceleration(environment.Gravity, simulationTime.Step);

            return State with
            {
                Velocity = newVelocity,
                Coordinates = State.Coordinates with
                {
                    X = State.Coordinates.X + newVelocity.X * timeInSeconds,
                    Y = State.Coordinates.Y + newVelocity.Y * timeInSeconds
                }
            };
        }

        public void Decide(ISimulationContext<BallsContainer> simulationContext)
        {
        }
    }

    public record BallBounce(double time, double place);

    public record BallCoordinates(double X, double Y)
    {
        internal double DistanceFrom(BallCoordinates coordinates)
        {
            return Math.Sqrt(Math.Pow(X - coordinates.X, 2) + Math.Pow(Y - coordinates.Y, 2));
        }

        internal (double deltaX, double deltaY) GetDirectionTo(BallCoordinates coordinates)
        {
            var distance = DistanceFrom(coordinates);
            var deltaX = (coordinates.X - X) / distance;
            var deltaY = (coordinates.Y - Y) / distance;

            return (deltaX, deltaY);
        }
    }

    public record BallVelocity(double X, double Y)
    {
        public double Value => Math.Sqrt(X * X + Y * Y);
        public BallVelocity ApplyAcceleration(BallAcceleration acceleration, TimeSpan deltaTime)
        {
            var timeInSecond = deltaTime.TotalSeconds;
            return this with { X = X + acceleration.X * timeInSecond, Y = Y + acceleration.Y * timeInSecond };
        }

        internal BallVelocity ChangeDirection(double deltaX, double deltaY)
        {
            var normalizationFactor = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            var powerFactor = Math.Abs(Value);
            var scaleFactor = powerFactor / normalizationFactor;

            return new BallVelocity(deltaX * scaleFactor, deltaY * scaleFactor);
        }

        internal BallVelocity Scale(double scale)
        {
            return new BallVelocity(X*scale, Y*scale);
        }
    }

    public record BallAcceleration(double X, double Y)
    {
        public double Value => Math.Sqrt(X * X + Y * Y);

        internal BallAcceleration Scale(double scale)
        {
            return new BallAcceleration(X * scale, Y * scale);
        }

        public static BallAcceleration operator +(BallAcceleration a, BallAcceleration b)
        {
            return new BallAcceleration(a.X + b.X, a.Y + b.Y);
        }
    }
}

using PKWat.AgentSimulation.Core;
using System;

namespace PKWat.AgentSimulations.Examples.CollisionDetection
{
    class Ball : IAgent<BallsContainer>
    {
        private readonly IRandomNumbersGenerator _randomNumbersGenerator;

        public Ball(IRandomNumbersGenerator randomNumbersGenerator)
        {
            _randomNumbersGenerator = randomNumbersGenerator;
        }

        private BallCoordinates _nextCoordinates;
        private BallVelocity _nextVelocity;

        public BallCoordinates Coordinates { get; private set; }
        public BallVelocity Velocity { get; private set; }

        public void Initialize(ISimulationContext<BallsContainer> simulationContext)
        {
            var environment = simulationContext.SimulationEnvironment;
            var x = _randomNumbersGenerator.NextDouble() * environment.Width;
            Coordinates = new BallCoordinates(x, environment.Height);
            Velocity = new BallVelocity(0, 0);
        }

        public void Act(ISimulationContext<BallsContainer> simulationContext)
        {
            Coordinates = _nextCoordinates;
            Velocity = _nextVelocity;
        }

        public void Decide(ISimulationContext<BallsContainer> simulationContext)
        {
            var environment = simulationContext.SimulationEnvironment;
            var timeInSeconds = simulationContext.SimulationStep.TotalSeconds;

            var timeToBounceLeft = CalculateTimeToBounce(Velocity.X, environment.Gravity.X, Coordinates.X, 0);
            var timeToBounceRight = CalculateTimeToBounce(Velocity.X, environment.Gravity.X, Coordinates.X, environment.Width);
            var timeToBounceBottom = CalculateTimeToBounce(Velocity.Y, environment.Gravity.Y, Coordinates.Y, 0);
            var timeToBounceTop = CalculateTimeToBounce(Velocity.Y, environment.Gravity.Y, Coordinates.Y, environment.Height);

            var timeToBounceVertical = SelectTimeToBounce(timeToBounceBottom, timeToBounceTop);
            var timeToBounceHorizontal = SelectTimeToBounce(timeToBounceLeft, timeToBounceRight);

            var newY = Coordinates.Y;
            var newVelocityY = Velocity.Y;
            if(timeToBounceVertical > timeInSeconds)
            {
                newY = Coordinates.Y 
                        + Velocity.Y * timeInSeconds
                        + environment.Gravity.Y * timeInSeconds * timeInSeconds / 2;
                newVelocityY = Velocity.Y + environment.Gravity.Y * timeInSeconds;
            }
            else
            {
                // Bounce

            }

            var newX = Coordinates.X;
            var newVelocityX = Velocity.X;
            if (timeToBounceHorizontal > timeInSeconds)
            {
                newX = Coordinates.X
                        + Velocity.X * timeInSeconds
                        + environment.Gravity.X * timeInSeconds * timeInSeconds / 2;
                newVelocityX = Velocity.X + environment.Gravity.X * timeInSeconds;
            }
            else
            {
                // Bounce

            }

            
            _nextCoordinates = new BallCoordinates(newX, newY);
            _nextVelocity = new BallVelocity(newVelocityX, newVelocityY);
        }

        private double CalculateTimeToBounce(double velocity, double acceleration, double position, double bouncePlace)
        {
            var distance = bouncePlace - position;
            var delta = velocity * velocity + 2 * acceleration * distance;
            if(delta < 0)
            {
                return -1;
            }

            var deltaSqrt = Math.Sqrt(delta);
            var t1 = (-velocity - deltaSqrt) / acceleration;
            var t2 = (-velocity + deltaSqrt) / acceleration;

            return SelectTimeToBounce(t1, t2);

        }

        private double SelectTimeToBounce(params double[] times)
        {
            return times.Where(t => t > 0).DefaultIfEmpty(double.MaxValue).Min();
        }
    }

    public record BallCoordinates(double X, double Y);
    public record BallVelocity(double X, double Y)
    {
        public BallVelocity ApplyAcceleration(BallAcceleration acceleration)
        {
            return this with { X = X + acceleration.X, Y = Y + acceleration.Y };
        }
    }

    public record BallAcceleration(double X, double Y);
}

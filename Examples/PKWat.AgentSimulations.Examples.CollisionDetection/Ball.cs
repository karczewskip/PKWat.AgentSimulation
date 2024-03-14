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
            Coordinates = new BallCoordinates(x, environment.Height/2);
            Velocity = new BallVelocity(50, 0);
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
            if(timeToBounceVertical?.time > timeInSeconds)
            {
                newY = Coordinates.Y 
                        + Velocity.Y * timeInSeconds
                        + environment.Gravity.Y * timeInSeconds * timeInSeconds / 2;
                newVelocityY = Velocity.Y + environment.Gravity.Y * timeInSeconds;
            }
            else if(timeToBounceVertical != null)
            {
                var timeAfterBounce = timeInSeconds - timeToBounceVertical.time;
                var velocityYAfterBounce = -(Velocity.Y + environment.Gravity.Y * timeAfterBounce);
                newY = velocityYAfterBounce * timeAfterBounce
                        + environment.Gravity.Y * timeAfterBounce * timeAfterBounce / 2;
                newVelocityY = velocityYAfterBounce + environment.Gravity.Y * timeAfterBounce;

            }

            var newX = Coordinates.X;
            var newVelocityX = Velocity.X;
            if (timeToBounceHorizontal?.time > timeInSeconds)
            {
                newX = Coordinates.X
                        + Velocity.X * timeInSeconds
                        + environment.Gravity.X * timeInSeconds * timeInSeconds / 2;
                newVelocityX = Velocity.X + environment.Gravity.X * timeInSeconds;
            }
            else if (timeToBounceHorizontal != null)
            {
                var timeAfterBounce = timeInSeconds - timeToBounceHorizontal.time;
                var velocityXAfterBounce = -(Velocity.X + environment.Gravity.X * timeAfterBounce);
                newX = timeToBounceHorizontal.place + velocityXAfterBounce * timeAfterBounce
                        + environment.Gravity.X * timeAfterBounce * timeAfterBounce / 2;
                newVelocityX = velocityXAfterBounce + environment.Gravity.X * timeAfterBounce;

            }

            
            _nextCoordinates = new BallCoordinates(newX, newY);
            _nextVelocity = new BallVelocity(newVelocityX, newVelocityY);
        }

        private BallBounce CalculateTimeToBounce(double velocity, double acceleration, double position, double bouncePlace)
        {
            var distance = bouncePlace - position;

            if(Math.Abs(acceleration) < 0.00001)
            {
                var time = distance / velocity;
                return new BallBounce(time > 0 ? time : double.MaxValue, bouncePlace);
            }

            var delta = velocity * velocity + 2 * acceleration * distance;
            if(delta < 0)
            {
                return new BallBounce(double.MaxValue, bouncePlace);
            }

            var deltaSqrt = Math.Sqrt(delta);
            var t1 = (-velocity - deltaSqrt) / acceleration;
            var t2 = (-velocity + deltaSqrt) / acceleration;

            return SelectTimeToBounce(
                new BallBounce(t1, bouncePlace), 
                new BallBounce(t2, bouncePlace));

        }

        private BallBounce SelectTimeToBounce(params BallBounce[] times)
        {
            return times.Where(e => e?.time > 0).OrderBy(x => x.time).FirstOrDefault();
        }
    }

    public record BallBounce(double time, double place);

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

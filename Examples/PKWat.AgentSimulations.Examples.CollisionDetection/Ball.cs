using PKWat.AgentSimulation.Core;
using System.Drawing;

namespace PKWat.AgentSimulations.Examples.CollisionDetection
{
    class Ball : IAgent<BallsContainer>
    {
        private readonly IRandomNumbersGenerator _randomNumbersGenerator;
        private readonly ColorInitializer _colorInitializer;

        public Ball(IRandomNumbersGenerator randomNumbersGenerator, ColorInitializer colorInitializer)
        {
            _randomNumbersGenerator = randomNumbersGenerator;
            _colorInitializer = colorInitializer;
        }

        private BallCoordinates _nextCoordinates;
        private BallVelocity _nextVelocity;

        public BallCoordinates Coordinates { get; private set; }
        public BallVelocity Velocity { get; private set; }
        public double Radius { get; } = 4.0;
        public Color Color { get; private set; } 

        public void Initialize(ISimulationContext<BallsContainer> simulationContext)
        {
            var environment = simulationContext.SimulationEnvironment;
            var x = _randomNumbersGenerator.NextDouble() * environment.Width;
            var y = environment.Height * 0.1 + _randomNumbersGenerator.NextDouble() * environment.Height/2;
            Coordinates = new BallCoordinates(x, y);
            Velocity = new BallVelocity(_randomNumbersGenerator.NextDouble()*50 - 25, _randomNumbersGenerator.NextDouble()*50 - 25);
            Color = _colorInitializer.GetNext();
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

            var ballAcceleration = environment.Gravity;

            var velocity = Velocity;
            var overlappingBalls = simulationContext.GetAgents<Ball>().Select(anotherBall => (Overlap: anotherBall.OverlapDistance(this), AnotherBall: anotherBall)).Where(x => x.Overlap > 0 && x.AnotherBall != this).ToArray();
            if(overlappingBalls.Any())
            {
                var delta = overlappingBalls.Aggregate((deltaX: 0.0,deltaY: 0.0), (accumulatedDelta, next) =>
                {
                    var nextDelta = next.AnotherBall.Coordinates.GetDirectionTo(Coordinates);
                    return (accumulatedDelta.deltaX + nextDelta.deltaX, accumulatedDelta.deltaY + nextDelta.deltaY);
                } );

                velocity = Velocity.ChangeDirection(delta.deltaX, delta.deltaY).Scale(0.5);
            }



            //foreach (var anotherBall in simulationContext.GetAgents<Ball>().Where(x => x != this))
            //{
            //    var overlapDistance = anotherBall.OverlapDistance(this);
            //    if (overlapDistance > 0)
            //    {
            //        var distance = anotherBall.Coordinates.DistanceFrom(Coordinates);
            //        var ballBouncingAccelerationValue = 500 / (distance*distance);
            //        var accelerationDirection = anotherBall.Coordinates.GetDirectionTo(Coordinates);
            //        var ballBouncingAcceleration = new BallAcceleration(accelerationDirection.deltaX * ballBouncingAccelerationValue, accelerationDirection.deltaY * ballBouncingAccelerationValue);
            //        ballAcceleration += ballBouncingAcceleration;
            //    }
            //}

            var timeToBounceLeft = CalculateTimeToBounce(velocity.X, ballAcceleration.X, Coordinates.X, 0);
            var timeToBounceRight = CalculateTimeToBounce(velocity.X, ballAcceleration.X, Coordinates.X, environment.Width);
            var timeToBounceBottom = CalculateTimeToBounce(velocity.Y, ballAcceleration.Y, Coordinates.Y, 0);
            var timeToBounceTop = CalculateTimeToBounce(velocity.Y, ballAcceleration.Y, Coordinates.Y, environment.Height);

            var timeToBounceVertical = SelectTimeToBounce(timeToBounceBottom, timeToBounceTop);
            var timeToBounceHorizontal = SelectTimeToBounce(timeToBounceLeft, timeToBounceRight);

            var newY = Coordinates.Y;
            var newVelocityY = velocity.Y;
            if(timeToBounceVertical?.time > timeInSeconds)
            {
                newY = Coordinates.Y 
                        + velocity.Y * timeInSeconds
                        + ballAcceleration.Y * timeInSeconds * timeInSeconds / 2;
                newVelocityY = velocity.Y + ballAcceleration.Y * timeInSeconds;
            }
            else if(timeToBounceVertical != null)
            {
                var timeAfterBounce = timeInSeconds - timeToBounceVertical.time;
                var velocityYAfterBounce = -(velocity.Y + ballAcceleration.Y * timeAfterBounce);
                newY = velocityYAfterBounce * timeAfterBounce
                        + ballAcceleration.Y * timeAfterBounce * timeAfterBounce / 2;
                newVelocityY = velocityYAfterBounce + ballAcceleration.Y * timeAfterBounce;

            }

            var newX = Coordinates.X;
            var newVelocityX = velocity.X;
            if (timeToBounceHorizontal?.time > timeInSeconds)
            {
                newX = Coordinates.X
                        + velocity.X * timeInSeconds
                        + ballAcceleration.X * timeInSeconds * timeInSeconds / 2;
                newVelocityX = velocity.X + ballAcceleration.X * timeInSeconds;
            }
            else if (timeToBounceHorizontal != null)
            {
                var timeAfterBounce = timeInSeconds - timeToBounceHorizontal.time;
                var velocityXAfterBounce = -(velocity.X + ballAcceleration.X * timeAfterBounce);
                newX = timeToBounceHorizontal.place + velocityXAfterBounce * timeAfterBounce
                        + ballAcceleration.X * timeAfterBounce * timeAfterBounce / 2;
                newVelocityX = velocityXAfterBounce + ballAcceleration.X * timeAfterBounce;

            }

            
            _nextCoordinates = new BallCoordinates(newX, newY);
            _nextVelocity = new BallVelocity(newVelocityX, newVelocityY);
        }

        private double OverlapDistance(Ball anotherBall)
        {
            var overlapDistance = Radius + anotherBall.Radius - Coordinates.DistanceFrom(anotherBall.Coordinates);
            return overlapDistance > 0 ? overlapDistance : 0;
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
        public BallVelocity ApplyAcceleration(BallAcceleration acceleration)
        {
            return this with { X = X + acceleration.X, Y = Y + acceleration.Y };
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
        public static BallAcceleration operator +(BallAcceleration a, BallAcceleration b)
        {
            return new BallAcceleration(a.X + b.X, a.Y + b.Y);
        }
    }
}

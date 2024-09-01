using PKWat.AgentSimulation.Core;

namespace PKWat.AgentSimulation.Examples.LiquidWpf.Simulation
{
    public class Drop(IRandomNumbersGenerator randomNumbersGenerator) : SimulationAgent<BinEnvironment, DropState>
    {
        protected override DropState GetInitialState(BinEnvironment environment)
        {
            var binHeight = environment.BinHeight;
            var binWidth = environment.BinWidth;

            var x = binWidth * randomNumbersGenerator.NextDouble();
            var y = binHeight / 2;

            return new DropState(new DropCoordinates(x, y), new DropVelocity(0, 0));
        }

        protected override DropState GetNextState(BinEnvironment environment, SimulationTime simulationTime)
        {
            var timeInSeconds = simulationTime.Step.TotalSeconds;

            var newVelocity = State.Velocity.ApplyAcceleration(environment.Gravity, simulationTime.Step);

            var newPosition = State.Position with
            {
                X = State.Position.X + newVelocity.X * timeInSeconds,
                Y = State.Position.Y + newVelocity.Y * timeInSeconds
            };

            return new DropState(newPosition, newVelocity);
        }
    }

    public record DropState(DropCoordinates Position, DropVelocity Velocity);

    public record DropCoordinates(double X, double Y);

    public record DropVelocity(double X, double Y)
    {
        public DropVelocity ApplyAcceleration(DropAcceleration acceleration, TimeSpan time)
        {
            return this with
            {
                X = X + acceleration.X * time.TotalSeconds,
                Y = Y + acceleration.Y * time.TotalSeconds
            };
        }
    }


    public record DropAcceleration(double X, double Y);
}

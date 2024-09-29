namespace PKWat.AgentSimulation.Examples.Airport.Simulation
{
    using PKWat.AgentSimulation.Core;
    using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;
    using PKWat.AgentSimulation.Examples.Airport.Simulation.Events;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    public class AirportSimulationBuilder
    {
        private readonly ISimulationBuilder _simulationBuilder;
        private readonly AirportDrawer _airportDrawer;

        public AirportSimulationBuilder(
            ISimulationBuilder simulationBuilder, 
            AirportDrawer airportDrawer)
        {
            _simulationBuilder = simulationBuilder;
            _airportDrawer = airportDrawer;
        }

        public ISimulation Build(Action<BitmapSource> drawing)
        {
            _airportDrawer.InitializeIfNeeded(800, 800);

            var simulation = _simulationBuilder
                .CreateNewSimulation(new AirportEnvironment())
                .AddAgent<Coordinator>()
                .AddEvent<NewAirplaneArrived>()
                .AddEnvironmentUpdates(UpdateAskingForLand)
                .AddEnvironmentUpdates(UpdateLandingAirplane)
                .AddEnvironmentUpdates(UpdateAllowedForLand)
                .AddEnvironmentUpdates(UpdateLandedAirplanes)
                .AddEnvironmentUpdates(UpdateNumberOfPassangersInEachAirplane)
                .AddCallback(c => RenderAsync(c, drawing))
                .SetSimulationStep(TimeSpan.FromMinutes(1))
                .SetWaitingTimeBetweenSteps(TimeSpan.FromSeconds(0.1))
                .SetRandomSeed(100)
                .Build();

            return simulation;
        }


        private async Task UpdateAskingForLand(ISimulationContext<AirportEnvironment> context)
        {
            context
                .SimulationEnvironment
                .SetAirplanesAskingForLand(context
                    .GetAgents<Airplane>()
                    .Where(x => x.State.AskingForLand)
                    .Select(x => x.Id)
                    .ToArray());
        }

        private async Task UpdateLandingAirplane(ISimulationContext<AirportEnvironment> context)
        {
            context
                .SimulationEnvironment
                .SetLandingAirplane(context
                    .GetAgents<Airplane>()
                    .Where(x => x.State.IsLanding(context.SimulationTime.Time))
                    .ToDictionary(x => x.Id, x => x.State.LandingLine.Value));
        }

        private async Task UpdateAllowedForLand(ISimulationContext<AirportEnvironment> context)
        {
            context
                .SimulationEnvironment
                .SetAllowedForLand(context
                    .GetRequiredAgent<Coordinator>()
                    .State
                    .AllowedAirplanesForLanding);
        }

        private async Task UpdateLandedAirplanes(ISimulationContext<AirportEnvironment> context)
        {
            context
                .SimulationEnvironment
                .SetLandedAirplanes(context
                    .GetAgents<Airplane>()
                    .Where(x => x.State.HasLanded(context.SimulationTime.Time) && !x.State.HasDeparted(context.SimulationTime.Time))
                    .ToDictionary(x => x.Id, x => x.State.LandingLine.Value));
        }

        private async Task UpdateNumberOfPassangersInEachAirplane(ISimulationContext<AirportEnvironment> context)
        {
            context
                .SimulationEnvironment
                .SetPassengersInEachAirplane(context
                    .GetAgents<Passenger>()
                    .Where(x => !x.State.Checkouted(context.SimulationTime.Time))
                    .GroupBy(x => x.State.AirplaneId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Id).ToArray()));
        }

        private async Task RenderAsync(ISimulationContext<AirportEnvironment> context, Action<BitmapSource> drawing)
            => drawing(_airportDrawer.Draw(context));
    }
}

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Events
{
    public class NewAirplaneArrived : ISimulationEvent<AirportEnvironment>
    {
        private TimeSpan _nextExecutingTime;

        public NewAirplaneArrived()
        {
            _nextExecutingTime = TimeSpan.FromMinutes(10);
        }

        public async Task Execute(ISimulationContext<AirportEnvironment> context)
        {
            context.AddAgent<Airplane>();
            _nextExecutingTime += TimeSpan.FromMinutes(10);
        }

        public Task<bool> ShouldBeExecuted(ISimulationContext<AirportEnvironment> context)
        {
            return Task.FromResult(context.SimulationTime >= _nextExecutingTime);
        }
    }
}

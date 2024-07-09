namespace PKWat.AgentSimulation.Examples.Airport.Simulation.Events;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;
using System;
using System.Threading.Tasks;

public class NewAirplaneArrived : ISimulationEvent<AirportEnvironment>
{
    private IRandomNumbersGenerator _randomNumbersGenerator;

    public NewAirplaneArrived(IRandomNumbersGenerator randomNumbersGenerator)
    {
        _randomNumbersGenerator = randomNumbersGenerator;

        _nextExecutingTime = GenerateTimeForNextExecution();
    }

    private TimeSpan _nextExecutingTime;

    public async Task Execute(ISimulationContext<AirportEnvironment> context)
    {
        var airplane = context.AddAgent<Airplane>();
        
        var passengers = _randomNumbersGenerator.Next(30);
        for(int i = 0; i < passengers; i++)
        {
            var passanger = context.AddAgent<Passenger>();
            passanger.SetAirplane(airplane.Id);
        }

        _nextExecutingTime += GenerateTimeForNextExecution();
    }

    public Task<bool> ShouldBeExecuted(ISimulationContext<AirportEnvironment> context)
    {
        return Task.FromResult(context.SimulationTime.Time >= _nextExecutingTime);
    }

    private TimeSpan GenerateTimeForNextExecution()
        => TimeSpan.FromMinutes(_randomNumbersGenerator.GetNextExponential(1.0 / 10.0));
}

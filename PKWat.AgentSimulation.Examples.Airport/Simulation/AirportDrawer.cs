namespace PKWat.AgentSimulation.Examples.Airport.Simulation;

using PKWat.AgentSimulation.Core;
using System.Drawing;
using System.Windows.Media.Imaging;
using PKWat.AgentSimulation.Drawing;
using PKWat.AgentSimulation.Examples.Airport.Simulation.Agents;
using System;

public class AirportDrawer
{
    private const int AirplaneSize = 10;

    private Bitmap _bmp;

    public void Initialize(int width, int height)
    {
        _bmp = new Bitmap(width, height);
        _bmp.SetResolution(96, 96);
    }

    public BitmapSource Draw(ISimulationContext<AirportEnvironment> context)
    {
        var waitingCordinates = new DrawingCoordinates(_bmp.Width / 2, 100);
        var landingPlace = new DrawingCoordinates(100, _bmp.Height - 100);

        using var graphic = Graphics.FromImage(_bmp);
        graphic.Clear(Color.White);

        var now = context.SimulationTime.Time;

        if(context.SimulationEnvironment.IsLandingAirplane)
        {
            var airplane = context.GetAgents<Airplane>().First(x => x.State.IsLanding(now));
            var coordinates = GetCoordinatesForLandingAirplane(airplane, waitingCordinates, landingPlace, now);

            graphic.FillEllipse(Brushes.Blue, coordinates.X, coordinates.Y, AirplaneSize, AirplaneSize);
        }

        int i = 0;
        foreach (var airplane in context.GetAgents<Airplane>().Where(x => x.State.IsBeforeLanding(now)))
        {
            var coordinates = GetCoordinatesForWaitingAirplane(i, waitingCordinates);

            graphic.FillEllipse(Brushes.Red, coordinates.X, coordinates.Y, AirplaneSize, AirplaneSize);
            i++;
        }

        return _bmp.ConvertToBitmapSource();
    }

    private DrawingCoordinates GetCoordinatesForWaitingAirplane(int index, DrawingCoordinates waitingCordinates)
    {
        return new DrawingCoordinates(waitingCordinates.X + index * AirplaneSize, waitingCordinates.Y);
    }

    private DrawingCoordinates GetCoordinatesForLandingAirplane(Airplane airplane, DrawingCoordinates waitingCordinates, DrawingCoordinates landingPlace, TimeSpan now)
    {
        var lerp = waitingCordinates.Lerp(landingPlace, airplane.State.LandingProgress(now));
        return new DrawingCoordinates(lerp.X, lerp.Y);
    }

    private record DrawingCoordinates(int X, int Y)
    {
        internal DrawingCoordinates Lerp(DrawingCoordinates landingPlace, double landingProgress)
        {
            return new DrawingCoordinates(
                (int)(X + (landingPlace.X - X) * landingProgress),
                (int)(Y + (landingPlace.Y - Y) * landingProgress));
        }
    }
}

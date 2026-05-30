namespace Dose.Core.Library.Locations
{
    public interface ICustomRoom : ILocation
    {
        ICustomRoom Initialize(ILocationManager locationManager);
    }
}
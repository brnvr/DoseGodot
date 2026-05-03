namespace Dose.Core.Library.Locations
{
    public interface ILocationManager
    {
        ILocation? CurrentLocation { get; set; }
        ILocation? GoToNextLocation();
        ILocation? GoToNextLocation(LocationType type);
    }
}
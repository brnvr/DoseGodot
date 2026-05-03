using Dose.Core.Library.Location;

namespace Dose.Core.Library.Locations
{
    public interface ILocationManager
    {
        ILocation? CurrentLocation { get; set; }
        ILocation? GoToNextLocation(IPortal portal);
    }
}
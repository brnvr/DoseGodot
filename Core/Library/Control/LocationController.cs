using Dose.Core.Library.Location;
using Dose.Core.Library.Locations;

namespace Dose.Core.Library.Control
{
    public class LocationController(ILocationManager locationManager)
    {
        public ILocationManager LocationManager { get; } = locationManager;

        public void UpdateLocation<TPortal>(TPortal portal, LocationType? type) where TPortal : IPortal
        {
            if (portal.Linked is null)
            {
                var location = type is null ? LocationManager.GoToNextLocation() : LocationManager.GoToNextLocation((LocationType)type);
    
                if (location != null)
                {
                    var other = location.SelectRandomPortal<TPortal>();
                
                    if (other != null)
                    {
                        portal.Link(other);
                    }
                }
            }
            else
            {
                LocationManager.CurrentLocation = portal.Linked.Location;
            }

            portal.Linked?.Location?.Clear(3);
        }
    }
}
using Dose.Core.Library.Location;
using Dose.Core.Library.Locations;

namespace Dose.Core.Library.Control
{
    public class LocationController(ILocationManager locationManager)
    {
        public ILocationManager LocationManager { get; } = locationManager;

        public void UpdateLocation<TPortal>(TPortal portal) where TPortal : IPortal
        {
            if (portal.Linked is null)
            {
                var location = LocationManager.GoToNextLocation(portal);
    
                if (location != null)
                {
                    var other = location.SelectRandomPortal<TPortal>(linkedPortal => LocationManager.PortalPredicate(portal, linkedPortal));
                
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
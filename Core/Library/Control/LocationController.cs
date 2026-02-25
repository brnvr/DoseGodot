using Dose.Core.Library.Locations;

namespace Dose.Core.Library.Control
{
    public class LocationController(ILocationManager locationManager)
    {
        public ILocationManager LocationManager { get; } = locationManager;

        public void UpdateLocation(IDoor door)
        {
            if (door.LinkedDoor is null)
            {
                var location = LocationManager.GoToNextLocation();
    
                if (location != null)
                {
                    var otherDoor = location.SelectRandomDoor();
                
                    if (otherDoor != null)
                    {
                        door.LinkDoor(otherDoor);
                    }
                }
            }
            else
            {
                LocationManager.CurrentLocation = door.LinkedDoor.Location;
            }

            door.LinkedDoor?.Location?.Clear(3);
        }
    }
}
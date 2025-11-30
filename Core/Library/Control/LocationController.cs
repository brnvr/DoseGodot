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
                var location = LocationManager.CreateLocation();
                var otherDoor = location?.SelectRandomDoor();
                
                if (otherDoor != null)
                {
                    door.LinkDoor(otherDoor);
                }

                LocationManager.CurrentLocation = location;
            }
            else
            {
                LocationManager.CurrentLocation = door.LinkedDoor.Location;
            }

            door.LinkedDoor?.Location?.Clear(3);
        }
    }
}
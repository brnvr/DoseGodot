using Dose.Core.Library.Actors;
using Dose.Core.Library.Locations;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.Interactions
{
    public class OpenDoorInteraction<TInteractor>(TInteractor interactor, ILocationManager locationManager) : ToggleInteraction<TInteractor, DoorState>(interactor)
    {
        public ILocationManager LocationManager { get; } = locationManager;
    
        public override void Perform(IActor subject)
        {
            if (subject is IDoor door)
            {
                if (door.OtherLocation is null)
                {
                    var location = LocationManager.CreateLocation();

                    if (location != null)
                    {
                        door.OtherLocation = location;
                        LocationManager.CurrentLocation = location;
                    }
                }
                else
                {
                    LocationManager.CurrentLocation = door.OtherLocation;
                }

                base.Perform(subject);
            }
        }
    }
}
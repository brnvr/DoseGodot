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

                if (Interactor is Node3D interactorNode && door.LinkedDoor is Node3D doorNode)
                {
                    interactorNode.Position = doorNode.Position;
                }

                door.LinkedDoor?.Location?.Clear(3);

                base.Perform(subject);
            }
        }
    }
}
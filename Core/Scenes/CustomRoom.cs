using Dose.Core.ActorArgs;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Locations;
using Godot;

namespace Dose.Core.Scenes
{
    public partial class CustomRoom : Location, ICustomRoom
    {
        [Export]
        ClockDirection _doorClockDirection = ClockDirection.Clockwise;

        public override LocationType? Type { get => LocationType.Room; }

        public CustomRoom Initialize(ILocationManager locationManager)
        {

            foreach (var node in GetChildren())
            {
                if (node is not IInitializableActor<DoorContext, DoorArgs> door)
                {
                    continue;
                }

                door.Initialize(new DoorContext(), new DoorArgs
                {
                    Location = this,
                    OpenDirection = ClockDirection.Clockwise,
                    LocationManager = locationManager
                });
            }

            return this;
        }

        ICustomRoom ICustomRoom.Initialize(ILocationManager locationManager)
        {
            return Initialize(locationManager);
        }

    }
}

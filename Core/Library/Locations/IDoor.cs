using Dose.Core.Library.Actors;
using Godot;

namespace Dose.Core.Library.Locations
{
    public interface IDoor
    {
        ILocation? Location { get; }
        IDoor? LinkedDoor { get; }
        ClockDirection OpenDirection { get; }
        void LinkDoor(IDoor other);
        void Initialize(DoorContext context, ILocationManager locationManager, ILocation location, ClockDirection openDirection);
    }
}
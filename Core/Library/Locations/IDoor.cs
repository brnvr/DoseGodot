using Dose.Core.Library.Actors;

namespace Dose.Core.Library.Locations
{
    public interface IDoor
    {
        ILocation? Location { get; }
        IDoor? LinkedDoor { get; }
        void LinkDoor(IDoor other);
        void Initialize(DoorContext context, ILocationManager locationManager, ILocation location);
    }
}
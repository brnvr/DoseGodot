using Dose.Core.Library.Locations;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.ActorArgs
{
    public class DoorArgs
    {
        public required ILocationManager LocationManager { get; init; }
        public required ILocation Location { get; init; }
        public required ClockDirection OpenDirection { get; init; }
        public required LocationOccluderFactory<LocationOccluder>? LocationOccluderFactory { get; init; }
    }
}
using Dose.Core.Library.Locations;
using Godot;

namespace Dose.Core.ActorArgs
{
    public class DoorArgs
    {
        public required StandardMaterial3D? Material { get; init; } 
        public required ILocationManager LocationManager { get; init; }
        public required ILocation Location { get; init; }
        public required ClockDirection OpenDirection { get; init; }
    }
}
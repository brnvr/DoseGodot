using Godot;

namespace Dose.Core.Library.Locations
{
    public readonly struct CustomRoomLoadArgs
    {
        public required PackedScene Scene { get; init; }
        public required ILocationManager LocationManager { get; init; }
    }
}
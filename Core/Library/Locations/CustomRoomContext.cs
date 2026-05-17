using Godot;

namespace Dose.Core.Library.Locations
{
    public class CustomRoomContext
    {
        public required int RoomNumber { get; init; }
        public required PackedScene Scene { get; set; }
    }
}
using Godot;

namespace Dose.Core.Library.Locations
{
    public class CustomRoomContext
    {
        public required int RoomNumber { get; init; }
        public required IDoor Door { get; init; }
        public required ILocation Location { get; init; }
    }
}
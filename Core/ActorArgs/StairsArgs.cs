using Dose.Core.Library.Locations;
using Godot;

namespace Dose.Core.ActorArgs
{
    public class StairsArgs
    {
        public required ILocation Location { get; init; }
        public required StandardMaterial3D WallMaterial { get; init; }
        public required StandardMaterial3D BlackMaterial { get; init; }
    }
}

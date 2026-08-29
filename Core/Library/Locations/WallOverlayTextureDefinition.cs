using Godot;
using Dose.Core.Library.Common;
using Dose.Core.Library.Packs;

namespace Dose.Core.Library.Locations
{
    public class WallOverlayTextureDefinition : IPackResource
    {
        public required Texture2D Texture { get; init; }
        public Rarity Rarity { get; init; }
        public required string PackId { get; init; }
        public LocationRestriction LocationRestriction { get; init; } = LocationRestriction.AnyLocation;
        public float Scale { get; init; } = 1f;
        public bool TextureRepeat { get; init; } = false;
    }
}

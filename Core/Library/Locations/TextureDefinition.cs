using Dose.Core.Library.Common;
using Godot;
using Dose.Core.Library.Packs;

namespace Dose.Core.Library.Locations
{
    public class TextureDefinition : IPackResource
    {
        Texture2D? _texture;

        public required Rarity Rarity { get; init; }
        public required string TexturePath { get; init; }
        public required string PackId { get; init; }
        public LocationRestriction LocationRestriction { get; init; } = LocationRestriction.AnyLocation;
        public Texture2D Texture => _texture ??= GD.Load<Texture2D>(TexturePath);
    }
}

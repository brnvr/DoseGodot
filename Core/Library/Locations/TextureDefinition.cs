using Dose.Core.Library.Common;
using Godot;
using Dose.Core.Library.Packs;

namespace Dose.Core.Library.Locations
{
    public class TextureDefinition : IPackResource
    {
        public required Rarity Rarity { get; init; }
        public required string TexturePath { get; init; }
        public required string PackId { get; init; }
    }
}

using Dose.Core.Library.Common;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class TextureDefinition : IHasRarity
    {
        public required Rarity Rarity { get; init; }
        public required string TexturePath { get; init; }
    }
}
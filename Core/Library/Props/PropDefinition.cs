using Godot;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Common;
using Dose.Core.Library.Packs;

namespace Dose.Core.Library.Props
{
    public class PropDefinition : IPackResource
    {
        public Texture2D? Texture { get; init; }
        public PackedScene? ModelScene { get; init; }
        public Rarity Rarity { get; init; }
        public required string PackId { get; init; }
        public LocationRestriction LocationRestriction { get; init; } = LocationRestriction.AnyLocation;
        public ScaleFactor Scale { get; init; } = 1f;
        public float YOffset { get; init; }
        public NormalizedValue MinDistanceFromWall { get; init; } = 0f;
        public NormalizedValue MaxDistanceFromWall { get; init; } = 1f;
        public NormalizedValue MinDistanceFromCorner { get; init; } = 0f;
        public NormalizedValue MaxDistanceFromCorner { get; init; } = 1f;
        public bool IsBillboard => Texture != null;
    }
}

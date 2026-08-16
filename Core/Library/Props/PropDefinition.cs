using Godot;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Common;

namespace Dose.Core.Library.Props
{
    public class PropDefinition : IHasRarity
    {
        public Texture2D? Texture { get; init; }
        public PackedScene? ModelScene { get; init; }
        public Rarity Rarity { get; init; }
        public ScaleFactor Scale { get; init; } = 1f;
        public NormalizedValue MinDistanceToWall { get; init; } = 0f;
        public NormalizedValue MaxDistanceToWall { get; init; } = 1f;
        public NormalizedValue MinDistanceFromCorner { get; init; } = 0f;
        public NormalizedValue MaxDistanceFromCorner { get; init; } = 1f;
        public bool IsBillboard => Texture != null;
    }
}

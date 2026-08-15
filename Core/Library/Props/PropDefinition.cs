using Godot;
using Dose.Core.Library.Numbers;

namespace Dose.Core.Library.Props
{
    public class PropDefinition
    {
        public Texture2D? Texture { get; init; }
        public PackedScene? ModelScene { get; init; }
        public NormalizedValue MinDistanceToWall { get; init; } = 0f;
        public NormalizedValue MaxDistanceToWall { get; init; } = 1f;
        public NormalizedValue MinDistanceFromCorner { get; init; } = 0f;
        public NormalizedValue MaxDistanceFromCorner { get; init; } = 1f;
        public bool IsBillboard => Texture != null;
    }
}

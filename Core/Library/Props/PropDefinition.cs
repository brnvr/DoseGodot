using Godot;

namespace Dose.Core.Library.Props
{
    public class PropDefinition
    {
        public Texture2D? Texture { get; init; }
        public PackedScene? ModelScene { get; init; }
        public bool IsBillboard => Texture != null;
    }
}

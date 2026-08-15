using Godot;

namespace Dose.Core.Library.Locations
{
    public class WallOverlayTextureDefinition
    {
        public required Texture2D Texture { get; init; }
        public float Scale { get; init; } = 1f;
        public bool TextureRepeat { get; init; } = false;
    }
}

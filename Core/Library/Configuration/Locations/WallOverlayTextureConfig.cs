using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Locations
{
    public class WallOverlayTextureConfig : YamlConfig
    {
        public required string TexturePath { get; init; }
        public required string Rarity { get; init; }
        public required string PackId { get; init; }
        public float Scale { get; init; } = 1f;
        public bool TextureRepeat { get; init; } = false;

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { "texturePath", TexturePath },
                { "rarity", Rarity },
                { "packId", PackId },
                { "scale", Scale },
                { "textureRepeat", TextureRepeat }
            };
        }
    }
}

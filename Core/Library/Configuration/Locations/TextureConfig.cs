using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Locations
{
    public class TextureConfig : YamlConfig
    {
        public required string Rarity { get; init; }
        public required string PackId { get; init; }
        public required string TexturePath { get; init; }

        public override Dictionary<string, object?> AsDict()
        {
            return new Dictionary<string, object?>
            {
                { "texturePath", TexturePath },
                { "rarity", Rarity.ToString() },
                { "packId", PackId }
            };
        }
    }
}

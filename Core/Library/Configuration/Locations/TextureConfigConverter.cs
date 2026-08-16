using System.Collections.Generic;

namespace Dose.Core.Library.Configuration.Locations
{
    public class TextureConfigConverter : YamlConfigConverter
    {
        public override TextureConfig FromDict(Dictionary<string, object?> dict)
        {
            return new TextureConfig
            {
                Rarity = GetRequired<string>(dict, "rarity"),
                PackId = GetRequired<string>(dict, "packId"),
                TexturePath = GetRequired<string>(dict, "texturePath")
            };
        }
    }
}

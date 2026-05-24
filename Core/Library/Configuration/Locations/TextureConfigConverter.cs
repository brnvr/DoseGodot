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
                TexturePath = GetRequired<string>(dict, "texturePath")
            };
        }
    }
}
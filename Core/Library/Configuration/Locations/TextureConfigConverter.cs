using System.Collections.Generic;
using Dose.Core.Library.Packs;

namespace Dose.Core.Library.Configuration.Locations
{
    public class TextureConfigConverter : YamlConfigConverter
    {
        public override TextureConfig FromDict(Dictionary<string, object?> dict)
        {
            return new TextureConfig
            {
                Rarity = GetRequired<string>(dict, nameof(TextureConfig.Rarity)),
                PackId = GetRequired<string>(dict, nameof(TextureConfig.PackId)),
                LocationRestriction = GetOptionalEnum(
                    dict,
                    nameof(TextureConfig.LocationRestriction),
                    LocationRestriction.AnyLocation
                ),
                TexturePath = GetRequired<string>(dict, nameof(TextureConfig.TexturePath))
            };
        }
    }
}

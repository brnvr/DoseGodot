using System.Collections.Generic;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Packs;

namespace Dose.Core.Library.Configuration.Locations
{
    public class LampTextureConfigConverter : YamlConfigConverter
    {
        public override LampTextureConfig FromDict(Dictionary<string, object?> dict)
        {
            return new LampTextureConfig
            {
                Rarity = GetRequired<string>(dict, nameof(LampTextureConfig.Rarity)),
                PackId = GetRequired<string>(dict, nameof(LampTextureConfig.PackId)),
                LocationRestriction = GetOptionalEnum(
                    dict,
                    nameof(LampTextureConfig.LocationRestriction),
                    LocationRestriction.AnyLocation
                ),
                TexturePath = GetRequired<string>(dict, nameof(LampTextureConfig.TexturePath)),
                Scale = new ScaleFactor(GetOptionalFloat(dict, nameof(LampTextureConfig.Scale), 1f))
            };
        }
    }
}

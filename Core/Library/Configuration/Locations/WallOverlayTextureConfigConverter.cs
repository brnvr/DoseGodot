using System;
using System.Collections.Generic;
using System.Globalization;
using Dose.Core.Library.Packs;

namespace Dose.Core.Library.Configuration.Locations
{
    public class WallOverlayTextureConfigConverter : YamlConfigConverter
    {
        public override WallOverlayTextureConfig FromDict(Dictionary<string, object?> dict)
        {
            var rawScale = GetOptional<object>(dict, nameof(WallOverlayTextureConfig.Scale)) ?? 1f;
            var scale = rawScale is string scaleText
                ? float.Parse(scaleText, NumberStyles.Float, CultureInfo.InvariantCulture)
                : Convert.ToSingle(rawScale, CultureInfo.InvariantCulture);
            var rawTextureRepeat = GetOptional<object>(dict, nameof(WallOverlayTextureConfig.TextureRepeat)) ?? false;
            var textureRepeat = Convert.ToBoolean(rawTextureRepeat, CultureInfo.InvariantCulture);

            if (scale <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "Wall overlay scale must be greater than zero.");
            }

            return new WallOverlayTextureConfig
            {
                TexturePath = GetRequired<string>(dict, nameof(WallOverlayTextureConfig.TexturePath)),
                Rarity = GetRequired<string>(dict, nameof(WallOverlayTextureConfig.Rarity)),
                PackId = GetRequired<string>(dict, nameof(WallOverlayTextureConfig.PackId)),
                LocationRestriction = GetOptionalEnum(
                    dict,
                    nameof(WallOverlayTextureConfig.LocationRestriction),
                    LocationRestriction.AnyLocation
                ),
                Scale = scale,
                TextureRepeat = textureRepeat
            };
        }
    }
}

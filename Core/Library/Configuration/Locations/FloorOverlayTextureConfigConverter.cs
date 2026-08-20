using System;
using System.Collections.Generic;
using System.Globalization;
using Dose.Core.Library.Packs;

namespace Dose.Core.Library.Configuration.Locations
{
    public class FloorOverlayTextureConfigConverter : YamlConfigConverter
    {
        public override FloorOverlayTextureConfig FromDict(Dictionary<string, object?> dict)
        {
            var rawScale = GetOptional<object>(dict, nameof(FloorOverlayTextureConfig.Scale)) ?? 1f;
            var scale = rawScale is string scaleText
                ? float.Parse(scaleText, NumberStyles.Float, CultureInfo.InvariantCulture)
                : Convert.ToSingle(rawScale, CultureInfo.InvariantCulture);
            var rawTextureRepeat = GetOptional<object>(dict, nameof(FloorOverlayTextureConfig.TextureRepeat)) ?? false;
            var textureRepeat = Convert.ToBoolean(rawTextureRepeat, CultureInfo.InvariantCulture);

            if (scale <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "Floor overlay scale must be greater than zero.");
            }

            return new FloorOverlayTextureConfig
            {
                TexturePath = GetRequired<string>(dict, nameof(FloorOverlayTextureConfig.TexturePath)),
                Rarity = GetRequired<string>(dict, nameof(FloorOverlayTextureConfig.Rarity)),
                PackId = GetRequired<string>(dict, nameof(FloorOverlayTextureConfig.PackId)),
                LocationRestriction = GetOptionalEnum(
                    dict,
                    nameof(FloorOverlayTextureConfig.LocationRestriction),
                    LocationRestriction.AnyLocation
                ),
                Scale = scale,
                TextureRepeat = textureRepeat
            };
        }
    }
}

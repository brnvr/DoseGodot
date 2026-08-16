using System;
using System.Collections.Generic;
using System.Globalization;

namespace Dose.Core.Library.Configuration.Locations
{
    public class WallOverlayTextureConfigConverter : YamlConfigConverter
    {
        public override WallOverlayTextureConfig FromDict(Dictionary<string, object?> dict)
        {
            var rawScale = GetOptional<object>(dict, "scale") ?? 1f;
            var scale = rawScale is string scaleText
                ? float.Parse(scaleText, NumberStyles.Float, CultureInfo.InvariantCulture)
                : Convert.ToSingle(rawScale, CultureInfo.InvariantCulture);
            var rawTextureRepeat = GetOptional<object>(dict, "textureRepeat") ?? false;
            var textureRepeat = Convert.ToBoolean(rawTextureRepeat, CultureInfo.InvariantCulture);

            if (scale <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "Wall overlay scale must be greater than zero.");
            }

            return new WallOverlayTextureConfig
            {
                TexturePath = GetRequired<string>(dict, "texturePath"),
                Rarity = GetRequired<string>(dict, "rarity"),
                PackId = GetRequired<string>(dict, "packId"),
                Scale = scale,
                TextureRepeat = textureRepeat
            };
        }
    }
}

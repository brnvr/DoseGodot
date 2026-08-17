using System;
using System.Collections.Generic;
using System.Globalization;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Packs;

namespace Dose.Core.Library.Configuration.Props
{
    public class PropConfigConverter : YamlConfigConverter
    {
        public override PropConfig FromDict(Dictionary<string, object?> dict)
        {
            var texturePath = GetOptional<string>(dict, nameof(PropConfig.TexturePath));
            var modelPath = GetOptional<string>(dict, nameof(PropConfig.ModelPath));
            var hasTexturePath = !string.IsNullOrWhiteSpace(texturePath);
            var hasModelPath = !string.IsNullOrWhiteSpace(modelPath);
            var minDistanceFromWall = GetNormalizedValue(dict, nameof(PropConfig.MinDistanceFromWall), 0f);
            var maxDistanceFromWall = GetNormalizedValue(dict, nameof(PropConfig.MaxDistanceFromWall), 1f);
            var minDistanceFromCorner = GetNormalizedValue(dict, nameof(PropConfig.MinDistanceFromCorner), 0f);
            var maxDistanceFromCorner = GetNormalizedValue(dict, nameof(PropConfig.MaxDistanceFromCorner), 1f);
            var scale = new ScaleFactor(GetOptionalFloat(dict, nameof(PropConfig.Scale), 1f));
            var yOffset = GetOptionalFloat(dict, nameof(PropConfig.YOffset), 0f);
            var surfaceHeight = GetOptionalSurfaceHeight(dict);

            if (hasTexturePath == hasModelPath)
            {
                throw new ArgumentException(
                    $"A prop config must define exactly one of {nameof(PropConfig.TexturePath)} or {nameof(PropConfig.ModelPath)}."
                );
            }

            if (!float.IsFinite(yOffset))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(PropConfig.YOffset),
                    $"A prop config's {nameof(PropConfig.YOffset)} must be finite."
                );
            }

            if (minDistanceFromWall.Value > maxDistanceFromWall.Value)
            {
                throw new ArgumentException(
                    $"A prop config's {nameof(PropConfig.MinDistanceFromWall)} cannot exceed {nameof(PropConfig.MaxDistanceFromWall)}."
                );
            }

            if (minDistanceFromCorner.Value > maxDistanceFromCorner.Value)
            {
                throw new ArgumentException(
                    $"A prop config's {nameof(PropConfig.MinDistanceFromCorner)} cannot exceed {nameof(PropConfig.MaxDistanceFromCorner)}."
                );
            }

            return new PropConfig
            {
                TexturePath = hasTexturePath ? texturePath : null,
                ModelPath = hasModelPath ? modelPath : null,
                Rarity = GetRequired<string>(dict, nameof(PropConfig.Rarity)),
                PackId = GetRequired<string>(dict, nameof(PropConfig.PackId)),
                LocationRestriction = GetOptionalEnum(
                    dict,
                    nameof(PropConfig.LocationRestriction),
                    LocationRestriction.AnyLocation
                ),
                Scale = scale,
                YOffset = yOffset,
                SurfaceHeight = surfaceHeight,
                MinDistanceFromWall = minDistanceFromWall,
                MaxDistanceFromWall = maxDistanceFromWall,
                MinDistanceFromCorner = minDistanceFromCorner,
                MaxDistanceFromCorner = maxDistanceFromCorner
            };
        }

        static float? GetOptionalSurfaceHeight(Dictionary<string, object?> dict)
        {
            var rawValue = GetOptional<object>(dict, nameof(PropConfig.SurfaceHeight));

            if (rawValue == null)
            {
                return null;
            }

            var value = rawValue is string valueText
                ? float.Parse(valueText, NumberStyles.Float, CultureInfo.InvariantCulture)
                : Convert.ToSingle(rawValue, CultureInfo.InvariantCulture);

            if (!float.IsFinite(value))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(PropConfig.SurfaceHeight),
                    $"A prop config's {nameof(PropConfig.SurfaceHeight)} must be finite."
                );
            }

            return value;
        }

        static NormalizedValue GetNormalizedValue(
            Dictionary<string, object?> dict,
            string key,
            float defaultValue
        )
        {
            var rawValue = GetOptional<object>(dict, key);

            if (rawValue == null)
            {
                return new NormalizedValue(defaultValue);
            }

            var value = rawValue is string valueText
                ? float.Parse(valueText, NumberStyles.Float, CultureInfo.InvariantCulture)
                : Convert.ToSingle(rawValue, CultureInfo.InvariantCulture);

            return new NormalizedValue(value);
        }
    }
}

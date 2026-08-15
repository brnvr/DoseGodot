using System;
using System.Collections.Generic;
using System.Globalization;
using Dose.Core.Library.Numbers;

namespace Dose.Core.Library.Configuration.Props
{
    public class PropConfigConverter : YamlConfigConverter
    {
        public override PropConfig FromDict(Dictionary<string, object?> dict)
        {
            var texturePath = GetOptional<string>(dict, "texturePath");
            var modelPath = GetOptional<string>(dict, "modelPath");
            var hasTexturePath = !string.IsNullOrWhiteSpace(texturePath);
            var hasModelPath = !string.IsNullOrWhiteSpace(modelPath);
            var minDistanceToWall = GetNormalizedValue(dict, "minDistanceToWall", 0f);
            var maxDistanceToWall = GetNormalizedValue(dict, "maxDistanceToWall", 1f);
            var minDistanceFromCorner = GetNormalizedValue(dict, "minDistanceFromCorner", 0f);
            var maxDistanceFromCorner = GetNormalizedValue(dict, "maxDistanceFromCorner", 1f);

            if (hasTexturePath == hasModelPath)
            {
                throw new ArgumentException(
                    "A prop config must define exactly one of texturePath or modelPath."
                );
            }

            if (minDistanceToWall.Value > maxDistanceToWall.Value)
            {
                throw new ArgumentException(
                    "A prop config's minDistanceToWall cannot exceed maxDistanceToWall."
                );
            }

            if (minDistanceFromCorner.Value > maxDistanceFromCorner.Value)
            {
                throw new ArgumentException(
                    "A prop config's minDistanceFromCorner cannot exceed maxDistanceFromCorner."
                );
            }

            return new PropConfig
            {
                TexturePath = hasTexturePath ? texturePath : null,
                ModelPath = hasModelPath ? modelPath : null,
                MinDistanceToWall = minDistanceToWall,
                MaxDistanceToWall = maxDistanceToWall,
                MinDistanceFromCorner = minDistanceFromCorner,
                MaxDistanceFromCorner = maxDistanceFromCorner
            };
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

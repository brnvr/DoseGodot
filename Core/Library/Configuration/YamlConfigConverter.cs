using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dose.Core.Library.Converters;

namespace Dose.Core.Library.Configuration
{
    public abstract class YamlConfigConverter : DictionaryConverter<YamlConfig>
    {
        public override Dictionary<string, object?> ToDict(YamlConfig obj)
        {
            return obj.AsDict();
        }

        protected static T? GetOptional<T>(Dictionary<string, object?> dict, string key)
        {
            dict.TryGetValue(key, out object? value);

            return (T?)value;
        }

        protected static T GetOptional<T>(Dictionary<string, object?> dict, string key, T def)
        {
            return GetOptional<T>(dict, key) ?? def;
        }

        protected static T GetRequired<T>(Dictionary<string, object?> dict, string key)
        {
            var value = GetOptional<T>(dict, key) ?? throw new Exception($"Dictionary doesn't contain required key \"{key}\".");

            return value;
        }

        protected static float GetOptionalFloat(
            Dictionary<string, object?> dict,
            string key,
            float defaultValue
        )
        {
            var rawValue = GetOptional<object>(dict, key);

            if (rawValue == null)
            {
                return defaultValue;
            }

            return rawValue is string valueText
                ? float.Parse(valueText, NumberStyles.Float, CultureInfo.InvariantCulture)
                : Convert.ToSingle(rawValue, CultureInfo.InvariantCulture);
        }

        protected static TEnum GetOptionalEnum<TEnum>(
            Dictionary<string, object?> dict,
            string key,
            TEnum defaultValue
        ) where TEnum : struct, Enum
        {
            var rawValue = GetOptional<object>(dict, key);

            if (rawValue == null)
            {
                return defaultValue;
            }

            return rawValue is TEnum enumValue
                ? enumValue
                : Enum.Parse<TEnum>(Convert.ToString(rawValue, CultureInfo.InvariantCulture)!, true);
        }
    }
}

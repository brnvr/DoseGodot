using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dose.Core.Library.Common;
using YamlDotNet.Serialization;

namespace Dose.Core.Library.Configuration
{
    public abstract class YamlConfig : IRepresentableAsDict<string, object?>
    {
        public abstract Dictionary<string, object?> AsDict();

        public string ToYaml()
        {
            var serializer = new SerializerBuilder().Build();

            var yaml = serializer.Serialize(AsDict());

            return yaml;
        }

        public void SaveYaml(string path)
        {
            File.WriteAllText(path, ToYaml());
        }

        public static object FromYaml(string path)
        {
            var deserializer = new DeserializerBuilder().Build();

            using var reader = new StreamReader(path);

            var raw = deserializer.Deserialize<object>(reader);

            return NormalizeYamlDict(raw);
        }

        static object NormalizeYamlDict(object input)
        {
            if (input is IDictionary<object, object> dict)
            {
                return dict.ToDictionary(
                    kvp => (string)kvp.Key,
                    kvp => NormalizeYamlDict(kvp.Value)
                );
            }

            if (input is IList list)
            {
                return list.Cast<object>()
                        .Select(NormalizeYamlDict)
                        .ToList();
            }

            return input;
        }
    }
}
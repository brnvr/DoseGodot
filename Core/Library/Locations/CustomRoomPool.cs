using System;
using System.Collections.Generic;
using System.IO;
using Dose.Core.Library.Configuration.Locations;
using YamlDotNet.Serialization;

namespace Dose.Core.Library.Locations
{
    public class CustomRoomPool(string configPath, CustomRoomConfigConverter configConverter, CustomRoomContextBuilder contextBuilder)
    {
        readonly List<CustomRoomContext> _availableContexts = [];

        public string ConfigPath { get; } = configPath;
        public List<CustomRoomContext> AvailableContexts => [.._availableContexts];
        public CustomRoomConfigConverter ConfigConverter { get; } = configConverter;
        public CustomRoomContextBuilder ContextBuilder { get; } = contextBuilder;

        public CustomRoomPool LoadContexts()
        {
            _availableContexts.Clear();
            var files = Directory.GetFiles(ConfigPath);
            var deserializer = new DeserializerBuilder().Build();

            foreach (var file in files)
            {
                Console.WriteLine($"Loading {file}...");

                var yaml = File.ReadAllText(file);
                var dict = deserializer.Deserialize<Dictionary<string, object?>>(yaml);
                var config = ConfigConverter.FromDict(dict);
                var context = ContextBuilder.BuildContext(config);

                if (context != null)
                {
                    _availableContexts.Add(context);
                }
            }

            return this;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using YamlDotNet.Serialization;

namespace Dose.Core.Library.DataStructures
{
    public class Pool<T>(
        string configPath,
        YamlConfigConverter configConverter,
        IObjectFromConfigBuilder<T> builder
    )
    {        
        readonly List<T> _content = [];

        public List<T> Content => [.. _content];
        public string ConfigPath { get; } = configPath;
        IObjectFromConfigBuilder<T> Builder { get; } = builder;
        public YamlConfigConverter ConfigConverter { get; } = configConverter;


        public Pool<T> Load()
        {
            _content.Clear();
            var files = Directory.GetFiles(ConfigPath);
            var deserializer = new DeserializerBuilder().Build();

            foreach (var file in files)
            {
                Console.WriteLine($"Loading {file}...");

                var yaml = File.ReadAllText(file);
                var dict = deserializer.Deserialize<Dictionary<string, object?>>(yaml);
                var config = ConfigConverter.FromDict(dict);
                var elem = Builder.Build(config);

                if (elem != null)
                {
                    _content.Add(elem);
                }
            }

            return this;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Dose.Core.Library.Configuration;
using Godot;
using YamlDotNet.Serialization;

namespace Dose.Core.Library.Actors
{
    public class ActorPool<TActorContext>(
        string configPath,
        PackedScene actorScene,
        YamlConfigConverter configConverter,
        IActorContextBuilder contextBuilder
    ) where TActorContext : IActorContext
    {
        readonly List<TActorContext> _avaliableContexts = [];

        public PackedScene ActorScene { get; } = actorScene;
        public string ConfigPath { get; } = configPath;
        public YamlConfigConverter ConfigConverter { get; } = configConverter;
        public IActorContextBuilder ContextBuilder { get; } = contextBuilder;
        public List<TActorContext> AvaliableContexts => [.._avaliableContexts];

        public ActorPool<TActorContext> LoadContexts()
        {
            _avaliableContexts.Clear();
            var files = Directory.GetFiles(ConfigPath);
            var deserializer = new DeserializerBuilder().Build();

            foreach (var file in files)
            {
                Console.WriteLine($"Loading {file}...");

                var yaml = File.ReadAllText(file);
                var dict = deserializer.Deserialize<Dictionary<string, object?>>(yaml);
                var config = ConfigConverter.FromDict(dict);
                var context = ContextBuilder.BuildContext(config);

                _avaliableContexts.Add((TActorContext)context);
            }

            return this;
        }
    }
}
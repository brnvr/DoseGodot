using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.DataStructures;
using Godot;

namespace Dose.Core.Library.Actors
{
    public class ActorPool<TActorContext>(
        string configPath,
        PackedScene actorScene,
        YamlConfigConverter configConverter,
        IObjectFromConfigBuilder<TActorContext> contextBuilder
    ) : WeightedPool<TActorContext>(configPath, configConverter, contextBuilder) where TActorContext : IActorContext, IHasRarity
    {
        public PackedScene ActorScene { get; } = actorScene;

        public new ActorPool<TActorContext> Load()
        {
            return (ActorPool<TActorContext>)base.Load();
        }
    }
}
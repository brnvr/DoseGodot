using System.Collections.Generic;
using Dose.Core.Library.Actors;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class LocationPopulatorSource<TActor, TActorContext>(PackedScene actorScene, IEnumerable<TActorContext> avaliableContexts)
        where TActor : class, IInitializableActor, new() where TActorContext : IActorContext
    {
        public PackedScene ActorScene { get; } = actorScene;
        public IEnumerable<TActorContext> AvaliableContexts { get; } = avaliableContexts;
    }
}
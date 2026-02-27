using System.Collections.Generic;
using Godot;

namespace Dose.Core.Library.Actors
{
    public class ActorPool<TActor, TActorContext, TArgs>(PackedScene actorScene, IEnumerable<TActorContext> avaliableContexts)
        where TActor : class, IInitializableActor<TActorContext, TArgs>, new() where TActorContext : IActorContext
    {
        public PackedScene ActorScene { get; } = actorScene;
        public IEnumerable<TActorContext> AvaliableContexts { get; } = avaliableContexts;
    }
}
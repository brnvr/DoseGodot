using System.Collections.Generic;
using Dose.Core.Library.Randomness;

namespace Dose.Core.Library.Actors
{
    public class ActorPoolSelector<TActor, TActorContext, TArgs>(ActorPool<TActorContext> source)
        where TActor : class, IInitializableActor<TActorContext, TArgs>, new() where TActorContext : IActorContext
    {
        readonly List<TActorContext> _availableContexts = [.. source.AvaliableContexts];
        readonly Dictionary<TActorContext, int> _contextQuantities = [];

        public ActorPool<TActorContext> Source { get; } = source;
        public List<TActorContext> AvaliableContexts => [.. _availableContexts];
        
        public TActor? Select(TArgs args)
        {
            if (_availableContexts.Count == 0) return default;

            var selector = new RandomSelector<TActorContext>(_availableContexts);

            var context = selector.Pick();

            if (context.MaxQuantity >= 0)
            {
                var quantity = _contextQuantities.TryGetValue(context, out var existingQuantity) ? existingQuantity : 0;

                if (quantity >= context.MaxQuantity)
                {
                    _availableContexts.Remove(context);
                    return Select(args);
                }

                _contextQuantities[context] = quantity + 1;
            }

            var actor = Source.ActorScene.Instantiate<TActor>();
            actor.Initialize(context, args);
            return actor;
        }
    }
}
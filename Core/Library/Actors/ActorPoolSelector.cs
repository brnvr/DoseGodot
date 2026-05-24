using System.Collections.Generic;
using Dose.Core.Library.Common;
using Dose.Core.Library.Randomness;

namespace Dose.Core.Library.Actors
{
    public class ActorPoolSelector<TActor, TActorContext, TArgs>(ActorPool<TActorContext> source)
        where TActor : class, IInitializableActor<TActorContext, TArgs>, new() where TActorContext : IActorContext, IHasRarity
    {
        readonly List<TActorContext> _content = [.. source.Content];
        readonly Dictionary<TActorContext, int> _contextQuantities = [];

        public ActorPool<TActorContext> Source { get; } = source;
        public List<TActorContext> Content => [.. _content];
        
        public TActor? Select(TArgs args)
        {
            if (Content.Count == 0) return default;

            var selector = new RandomSelector<TActorContext>(_content);

            var context = selector.Pick();

            if (context.MaxQuantity >= 0)
            {
                var quantity = _contextQuantities.TryGetValue(context, out var existingQuantity) ? existingQuantity : 0;

                if (quantity >= context.MaxQuantity)
                {
                    _content.Remove(context);
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
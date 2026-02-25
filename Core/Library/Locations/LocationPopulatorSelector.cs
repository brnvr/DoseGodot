using System.Collections.Generic;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Randomness;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class LocationPopulatorSelector<TActor, TActorContext>(LocationPopulatorSource<TActor, TActorContext> source)
        where TActor : class, IInitializableActor, new() where TActorContext : IActorContext
    {
        readonly List<TActorContext> _availableContexts = [.. source.AvaliableContexts];
        readonly Dictionary<TActorContext, int> _contextQuantities = [];

        public LocationPopulatorSource<TActor, TActorContext> Source { get; } = source;
        public List<TActorContext> AvaliableContexts => [.. _availableContexts];
        
        public TActor? Select()
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
                    return Select();
                }

                _contextQuantities[context] = quantity + 1;
            }

            var actor = Source.ActorScene.Instantiate<TActor>();
            actor.Initialize(context);
            return actor;
        }
    }
}
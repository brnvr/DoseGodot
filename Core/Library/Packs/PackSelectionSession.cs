using System.Collections.Generic;
using System.Linq;
using Dose.Core.ActorArgs;
using Dose.Core.Library.Actors;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.Randomness;
using Dose.Core.Library.Props;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.Library.Packs
{
    public class PackSelectionSession(
        Pack pack,
        PackedScene itemScene,
        PackedScene npcScene,
        PackedScene propScene,
        LocationRestriction locationRestriction
    )
    {
        readonly List<ItemContext> _items = [
            .. pack.GetWeightedItems()
                .Where(item => item.CanPopulate(locationRestriction))
                .Distinct()
        ];
        readonly List<NpcContext> _npcs = [
            .. pack.GetWeightedNpcs()
                .Where(npc => npc.CanPopulate(locationRestriction))
                .Distinct()
        ];
        readonly List<PropDefinition> _props = [
            .. pack.GetWeightedProps()
                .Where(prop => prop.CanPopulate(locationRestriction))
        ];
        readonly Dictionary<IActorContext, int> _quantities = [];

        public Pack Pack { get; } = pack;
        public PackedScene ItemScene { get; } = itemScene;
        public PackedScene NpcScene { get; } = npcScene;
        public PackedScene PropScene { get; } = propScene;

        public Item? SelectItem(ItemArgs args)
        {
            var context = SelectFinite(_items);

            if (context == null)
            {
                return null;
            }

            var item = ItemScene.Instantiate<Item>();
            item.Initialize(context, args);
            return item;
        }

        public Npc? SelectNpc(NpcArgs args)
        {
            var context = SelectFinite(_npcs);

            if (context == null)
            {
                return null;
            }

            var npc = NpcScene.Instantiate<Npc>();
            npc.Initialize(context, args);
            return npc;
        }

        public Prop? SelectProp()
        {
            if (_props.Count == 0)
            {
                return null;
            }

            var definition = new RandomSelector<PropDefinition>(_props).Pick();
            var prop = PropScene.Instantiate<Prop>();
            prop.Initialize(definition);
            return prop;
        }

        TContext? SelectFinite<TContext>(List<TContext> available)
            where TContext : class, IActorContext, Dose.Core.Library.Common.IHasRarity
        {
            available.RemoveAll(context => context.MaxQuantity == 0);

            if (available.Count == 0)
            {
                return null;
            }

            var weighted = new RarityWeighter<TContext>([.. available]).Weight();
            var context = new RandomSelector<TContext>(weighted).Pick();
            var quantity = _quantities.GetValueOrDefault(context) + 1;
            _quantities[context] = quantity;

            if (context.MaxQuantity > 0 && quantity >= context.MaxQuantity)
            {
                available.Remove(context);
            }

            return context;
        }
    }
}

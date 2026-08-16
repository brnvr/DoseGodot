using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.Locations;
using Dose.Core.Library.Props;
using Dose.Core.Library.Randomness;

namespace Dose.Core.Library.Packs
{
    public class Pack : IHasRarity
    {
        public string Id { get; }
        public Pack? Parent { get; }
        public Rarity Rarity { get; }
        public PackResourcePool<ItemContext> Items { get; }
        public PackResourcePool<NpcContext> Npcs { get; }
        public PackResourcePool<TextureDefinition> WallTextures { get; }
        public PackResourcePool<TextureDefinition> DoorTextures { get; }
        public PackResourcePool<TextureDefinition> LampTextures { get; }
        public PackResourcePool<WallOverlayTextureDefinition> WallOverlays { get; }
        public PackResourcePool<PropDefinition> Props { get; }

        public Pack(
            string id,
            Pack? parent,
            Rarity rarity,
            PackResourcePool<ItemContext> items,
            PackResourcePool<NpcContext> npcs,
            PackResourcePool<TextureDefinition> wallTextures,
            PackResourcePool<TextureDefinition> doorTextures,
            PackResourcePool<TextureDefinition> lampTextures,
            PackResourcePool<WallOverlayTextureDefinition> wallOverlays,
            PackResourcePool<PropDefinition> props
        )
        {
            Id = id;
            Parent = parent;
            Rarity = rarity;
            Items = items;
            Npcs = npcs;
            WallTextures = wallTextures;
            DoorTextures = doorTextures;
            LampTextures = lampTextures;
            WallOverlays = wallOverlays;
            Props = props;
        }

        public IEnumerable<ItemContext> GetWeightedItems() =>
            CombineWithParents(Items.GetWeightedList(), pack => pack.Items.GetWeightedList());

        public IEnumerable<NpcContext> GetWeightedNpcs() =>
            CombineWithParents(Npcs.GetWeightedList(), pack => pack.Npcs.GetWeightedList());

        public IEnumerable<TextureDefinition> GetWeightedWallTextures() =>
            CombineWithParents(WallTextures.GetWeightedList(), pack => pack.WallTextures.GetWeightedList());

        public IEnumerable<TextureDefinition> GetWeightedDoorTextures() =>
            CombineWithParents(DoorTextures.GetWeightedList(), pack => pack.DoorTextures.GetWeightedList());

        public IEnumerable<TextureDefinition> GetWeightedLampTextures() =>
            CombineWithParents(LampTextures.GetWeightedList(), pack => pack.LampTextures.GetWeightedList());

        public IEnumerable<WallOverlayTextureDefinition> GetWeightedWallOverlays() =>
            CombineWithParents(WallOverlays.GetWeightedList(), pack => pack.WallOverlays.GetWeightedList());

        public IEnumerable<PropDefinition> GetWeightedProps() =>
            CombineWithParents(Props.GetWeightedList(), pack => pack.Props.GetWeightedList());

        public ItemContext? SelectItem() => Select(GetWeightedItems());
        public NpcContext? SelectNpc() => Select(GetWeightedNpcs());
        public TextureDefinition? SelectWallTexture() => Select(GetWeightedWallTextures());
        public TextureDefinition? SelectDoorTexture() => Select(GetWeightedDoorTextures());
        public TextureDefinition? SelectLampTexture() => Select(GetWeightedLampTextures());
        public WallOverlayTextureDefinition? SelectWallOverlay() => Select(GetWeightedWallOverlays());
        public PropDefinition? SelectProp() => Select(GetWeightedProps());

        IEnumerable<T> CombineWithParents<T>(
            IEnumerable<T> localValues,
            System.Func<Pack, IEnumerable<T>> valuesFromPack
        )
        {
            var values = localValues;

            for (var parent = Parent; parent != null; parent = parent.Parent)
            {
                values = values.Concat(valuesFromPack(parent));
            }

            return values;
        }

        static T? Select<T>(IEnumerable<T> values) where T : class
        {
            var candidates = values.ToList();
            return candidates.Count == 0 ? null : new RandomSelector<T>(candidates).Pick();
        }
    }
}

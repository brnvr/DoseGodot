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
        public PackResourcePool<FloorOverlayTextureDefinition> FloorOverlays { get; }
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
            PackResourcePool<FloorOverlayTextureDefinition> floorOverlays,
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
            FloorOverlays = floorOverlays;
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

        public IEnumerable<FloorOverlayTextureDefinition> GetWeightedFloorOverlays() =>
            CombineWithParents(FloorOverlays.GetWeightedList(), pack => pack.FloorOverlays.GetWeightedList());

        public IEnumerable<PropDefinition> GetWeightedProps() =>
            CombineWithParents(Props.GetWeightedList(), pack => pack.Props.GetWeightedList());

        public ItemContext? SelectItem(LocationRestriction restriction) =>
            Select(GetWeightedItems().Where(item => item.CanPopulate(restriction)));

        public NpcContext? SelectNpc(LocationRestriction restriction) =>
            Select(GetWeightedNpcs().Where(npc => npc.CanPopulate(restriction)));

        public TextureDefinition? SelectWallTexture(LocationRestriction restriction) =>
            Select(GetWeightedWallTextures().Where(texture => texture.CanPopulate(restriction)));

        public TextureDefinition? SelectDoorTexture(LocationRestriction restriction) =>
            Select(GetWeightedDoorTextures().Where(texture => texture.CanPopulate(restriction)));

        public TextureDefinition? SelectLampTexture(LocationRestriction restriction) =>
            Select(GetWeightedLampTextures().Where(texture => texture.CanPopulate(restriction)));

        public WallOverlayTextureDefinition? SelectWallOverlay(LocationRestriction restriction) =>
            Select(GetWeightedWallOverlays().Where(overlay => overlay.CanPopulate(restriction)));

        public FloorOverlayTextureDefinition? SelectFloorOverlay(LocationRestriction restriction) =>
            Select(GetWeightedFloorOverlays().Where(overlay => overlay.CanPopulate(restriction)));

        public PropDefinition? SelectProp(LocationRestriction restriction) =>
            Select(GetWeightedProps().Where(prop => prop.CanPopulate(restriction)));

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

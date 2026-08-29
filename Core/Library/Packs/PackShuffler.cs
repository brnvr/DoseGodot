using System;
using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.Locations;
using Dose.Core.Library.Numbers;
using Dose.Core.Library.Props;
using Dose.Core.Library.Randomness;

namespace Dose.Core.Library.Packs
{
    public class PackShuffler(IEnumerable<Pack> packs, Probability randomness)
    {
        readonly List<Pack> _packs = [.. packs];

        public Probability Randomness { get; } = randomness;

        public Pack? Shuffle()
        {
            var mainPack = SelectMainPack();

            if (mainPack == null)
            {
                return null;
            }

            var resources = new ShuffledResources(mainPack);
            var donors = _packs
                .Where(pack => pack != mainPack)
                .Select(pack => new DonorResources(pack, mainPack))
                .Where(donor => donor.HasResources)
                .ToList();
            var decider = new RandomDecider(Randomness);

            while (donors.Count > 0 && decider.Roll())
            {
                var donor = new RandomSelector<DonorResources>(donors).Pick();
                donor.PullRandomResourceInto(resources);

                if (!donor.HasResources)
                {
                    donors.Remove(donor);
                }
            }

            return resources.HasImports ? resources.Build() : mainPack;
        }

        Pack? SelectMainPack()
        {
            if (_packs.Count == 0)
            {
                return null;
            }

            var weighted = new RarityWeighter<Pack>([.. _packs]).Weight();
            return new RandomSelector<Pack>(weighted).Pick();
        }

        sealed class DonorResources
        {
            readonly List<ItemContext> _items;
            readonly List<NpcContext> _npcs;
            readonly List<TextureDefinition> _wallTextures;
            readonly List<TextureDefinition> _doorTextures;
            readonly List<LampTextureDefinition> _lampTextures;
            readonly List<WallOverlayTextureDefinition> _wallOverlays;
            readonly List<FloorOverlayTextureDefinition> _floorOverlays;
            readonly List<PropDefinition> _props;

            public bool HasResources =>
                _items.Count > 0
                || _npcs.Count > 0
                || _wallTextures.Count > 0
                || _doorTextures.Count > 0
                || _lampTextures.Count > 0
                || _wallOverlays.Count > 0
                || _floorOverlays.Count > 0
                || _props.Count > 0;

            public DonorResources(Pack donor, Pack mainPack)
            {
                _items = ExceptAccessible(donor.Items.Content, mainPack.GetWeightedItems());
                _npcs = ExceptAccessible(donor.Npcs.Content, mainPack.GetWeightedNpcs());
                _wallTextures = ExceptAccessible(
                    donor.WallTextures.Content,
                    mainPack.GetWeightedWallTextures()
                );
                _doorTextures = ExceptAccessible(
                    donor.DoorTextures.Content,
                    mainPack.GetWeightedDoorTextures()
                );
                _lampTextures = ExceptAccessible(
                    donor.LampTextures.Content,
                    mainPack.GetWeightedLampTextures()
                );
                _wallOverlays = ExceptAccessible(
                    donor.WallOverlays.Content,
                    mainPack.GetWeightedWallOverlays()
                );
                _floorOverlays = ExceptAccessible(
                    donor.FloorOverlays.Content,
                    mainPack.GetWeightedFloorOverlays()
                );
                _props = ExceptAccessible(donor.Props.Content, mainPack.GetWeightedProps());
            }

            public void PullRandomResourceInto(ShuffledResources destination)
            {
                List<Action> availablePulls = [];

                AddPullIfAvailable(_items, destination.Items, availablePulls);
                AddPullIfAvailable(_npcs, destination.Npcs, availablePulls);
                AddPullIfAvailable(_wallTextures, destination.WallTextures, availablePulls);
                AddPullIfAvailable(_doorTextures, destination.DoorTextures, availablePulls);
                AddPullIfAvailable(_lampTextures, destination.LampTextures, availablePulls);
                AddPullIfAvailable(_wallOverlays, destination.WallOverlays, availablePulls);
                AddPullIfAvailable(_floorOverlays, destination.FloorOverlays, availablePulls);
                AddPullIfAvailable(_props, destination.Props, availablePulls);

                new RandomSelector<Action>(availablePulls).Pick()();
            }

            static List<T> ExceptAccessible<T>(
                IEnumerable<T> donorResources,
                IEnumerable<T> accessibleResources
            ) where T : notnull
            {
                var accessible = accessibleResources.ToHashSet();
                return [.. donorResources.Where(resource => !accessible.Contains(resource))];
            }

            static void AddPullIfAvailable<T>(
                List<T> source,
                List<T> destination,
                List<Action> availablePulls
            ) where T : notnull
            {
                if (source.Count == 0)
                {
                    return;
                }

                availablePulls.Add(() =>
                {
                    var resource = new RandomSelector<T>(source).Pick();
                    source.Remove(resource);
                    destination.Add(resource);
                });
            }
        }

        sealed class ShuffledResources(Pack mainPack)
        {
            public List<ItemContext> Items { get; } = [];
            public List<NpcContext> Npcs { get; } = [];
            public List<TextureDefinition> WallTextures { get; } = [];
            public List<TextureDefinition> DoorTextures { get; } = [];
            public List<LampTextureDefinition> LampTextures { get; } = [];
            public List<WallOverlayTextureDefinition> WallOverlays { get; } = [];
            public List<FloorOverlayTextureDefinition> FloorOverlays { get; } = [];
            public List<PropDefinition> Props { get; } = [];

            public bool HasImports =>
                Items.Count > 0
                || Npcs.Count > 0
                || WallTextures.Count > 0
                || DoorTextures.Count > 0
                || LampTextures.Count > 0
                || WallOverlays.Count > 0
                || FloorOverlays.Count > 0
                || Props.Count > 0;

            public Pack Build()
            {
                return new Pack(
                    mainPack.Id,
                    mainPack.Parent,
                    mainPack.Rarity,
                    CombinedPool(mainPack.Items, Items),
                    CombinedPool(mainPack.Npcs, Npcs),
                    CombinedPool(mainPack.WallTextures, WallTextures),
                    CombinedPool(mainPack.DoorTextures, DoorTextures),
                    CombinedPool(mainPack.LampTextures, LampTextures),
                    CombinedPool(mainPack.WallOverlays, WallOverlays),
                    CombinedPool(mainPack.FloorOverlays, FloorOverlays),
                    CombinedPool(mainPack.Props, Props)
                );
            }

            static PackResourcePool<T> CombinedPool<T>(
                PackResourcePool<T> mainPool,
                IEnumerable<T> imports
            ) where T : notnull, IPackResource
            {
                return new PackResourcePool<T>(mainPool.Content.Concat(imports));
            }
        }
    }
}

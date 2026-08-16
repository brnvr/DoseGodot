using System;
using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration.Packs;
using Dose.Core.Library.Locations;
using Dose.Core.Library.Props;
using Godot;

namespace Dose.Core.Library.Packs
{
    public class PackPoolBuilder(
        IEnumerable<PackConfig> configs,
        ActorPool<ItemContext> items,
        ActorPool<NpcContext> npcs,
        IEnumerable<TextureDefinition> wallTextures,
        IEnumerable<TextureDefinition> doorTextures,
        IEnumerable<TextureDefinition> lampTextures,
        IEnumerable<WallOverlayTextureDefinition> wallOverlays,
        PropPool props
    )
    {
        readonly List<PackConfig> _configs = [.. configs];
        readonly List<ItemContext> _items = [.. items.Content];
        readonly List<NpcContext> _npcs = [.. npcs.Content];
        readonly List<TextureDefinition> _wallTextures = [.. wallTextures];
        readonly List<TextureDefinition> _doorTextures = [.. doorTextures];
        readonly List<TextureDefinition> _lampTextures = [.. lampTextures];
        readonly List<WallOverlayTextureDefinition> _wallOverlays = [.. wallOverlays];
        readonly List<PropDefinition> _props = [.. props.Content];

        public PackPool Build()
        {
            var duplicateId = _configs
                .GroupBy(config => config.Id)
                .FirstOrDefault(group => group.Count() > 1)?.Key;

            if (duplicateId != null)
            {
                throw new InvalidOperationException($"Duplicate pack id: {duplicateId}.");
            }

            var configsById = _configs.ToDictionary(config => config.Id);
            ValidateResourcePackIds(configsById.Keys);

            var packsById = new Dictionary<string, Pack>();
            var resolving = new HashSet<string>();

            Pack Resolve(string id)
            {
                if (packsById.TryGetValue(id, out var existing))
                {
                    return existing;
                }

                if (!configsById.TryGetValue(id, out var config))
                {
                    throw new InvalidOperationException($"Pack '{id}' does not exist.");
                }

                if (!resolving.Add(id))
                {
                    throw new InvalidOperationException($"Pack inheritance cycle detected at '{id}'.");
                }

                var parent = config.ParentId == null ? null : Resolve(config.ParentId);
                var pack = new Pack(
                    config.Id,
                    parent,
                    Enum.Parse<Rarity>(config.Rarity, true),
                    PoolFor(_items, id),
                    PoolFor(_npcs, id),
                    PoolFor(_wallTextures, id),
                    PoolFor(_doorTextures, id),
                    PoolFor(_lampTextures, id),
                    PoolFor(_wallOverlays, id),
                    PoolFor(_props, id)
                );

                resolving.Remove(id);
                packsById.Add(id, pack);
                return pack;
            }

            foreach (var id in configsById.Keys)
            {
                Resolve(id);
            }

            return new PackPool(packsById.Values, items.ActorScene, npcs.ActorScene, props.PropScene);
        }

        void ValidateResourcePackIds(IEnumerable<string> packIds)
        {
            var validIds = packIds.ToHashSet();
            var invalidResource = AllResources().FirstOrDefault(resource => !validIds.Contains(resource.PackId));

            if (invalidResource != null)
            {
                throw new InvalidOperationException(
                    $"Resource references unknown pack '{invalidResource.PackId}'."
                );
            }
        }

        IEnumerable<IPackResource> AllResources()
        {
            return _items.Cast<IPackResource>()
                .Concat(_npcs)
                .Concat(_wallTextures)
                .Concat(_doorTextures)
                .Concat(_lampTextures)
                .Concat(_wallOverlays)
                .Concat(_props);
        }

        static PackResourcePool<T> PoolFor<T>(IEnumerable<T> resources, string packId)
            where T : notnull, IPackResource
        {
            return new PackResourcePool<T>(resources.Where(resource => resource.PackId == packId));
        }
    }
}

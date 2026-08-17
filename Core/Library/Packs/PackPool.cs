using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Common;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.Randomness;
using Godot;

namespace Dose.Core.Library.Packs
{
    public class PackPool(
        IEnumerable<Pack> packs,
        PackedScene itemScene,
        PackedScene npcScene,
        PackedScene propScene
    )
    {
        readonly List<Pack> _packs = [.. packs];

        public IReadOnlyList<Pack> Content => _packs;
        public PackedScene ItemScene { get; } = itemScene;
        public PackedScene NpcScene { get; } = npcScene;
        public PackedScene PropScene { get; } = propScene;

        public Pack? Select()
        {
            if (_packs.Count == 0)
            {
                return null;
            }

            var weighted = new RarityWeighter<Pack>([.. _packs]).Weight();
            return new RandomSelector<Pack>(weighted).Pick();
        }

        public PackSelectionSession CreateSelectionSession(
            Pack pack,
            LocationRestriction locationRestriction
        )
        {
            return new PackSelectionSession(
                pack,
                ItemScene,
                NpcScene,
                PropScene,
                locationRestriction
            );
        }
    }
}

using System.Collections.Generic;
using Dose.Core.Library.Numbers;
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
            return Select(new Probability(0));
        }

        public Pack? Select(Probability randomness) =>
            new PackShuffler(_packs, randomness).Shuffle();

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

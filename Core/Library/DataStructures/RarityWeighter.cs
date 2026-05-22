using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Common;

namespace Dose.Core.Library.DataStructures
{
    public class RarityWeighter<T>(params T[] items) where T : IHasRarity
    {
        IEnumerable<T> Items { get; } = items;

        public IEnumerable<T> Weight()
        {
            return new WeightedList<T>([.. Items.Select(item => (item, (int)item.Rarity))]);
        }
    }
}
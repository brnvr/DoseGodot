using System.Collections.Generic;
using Dose.Core.Library.DataStructures;

namespace Dose.Core.Library.Packs
{
    public class PackResourcePool<T>(IEnumerable<T> content) where T : notnull, IPackResource
    {
        readonly List<T> _content = [.. content];

        public IReadOnlyList<T> Content => _content;

        public WeightedList<T> GetWeightedList()
        {
            return new RarityWeighter<T>([.. _content]).Weight();
        }
    }
}

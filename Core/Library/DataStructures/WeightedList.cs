using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.DataStructures
{
    public class WeightedList<T>(params (T Value, int Weight)[] items) : IEnumerable<T> where T : notnull
    {
        private readonly Dictionary<T, int> _items = items.ToDictionary(i => i.Value, i => i.Weight);

        public void Add(T value, int weight)
        {
            _items[value] = weight;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items
                .SelectMany(kvp => Enumerable.Repeat(kvp.Key, kvp.Value))
                .GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
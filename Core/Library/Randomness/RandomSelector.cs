using System;

using System.Collections.Generic;
using System.Linq;

namespace Dose.Core.Library.Randomness
{
    public class RandomSelector<T>(IEnumerable<T> values)
    {
        readonly IList<T> _values = [.. values];

        public T Pick(int? seed = null)
        {
            var random = seed == null ? new Random() : new Random((int)seed);

            return _values[random.Next(_values.Count())];
        }
    }
}
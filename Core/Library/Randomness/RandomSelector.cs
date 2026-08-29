using System;

using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Common;

namespace Dose.Core.Library.Randomness
{
    public class RandomSelector<T>(IEnumerable<T> values)
    {
        readonly IList<T> _values = SelectCandidates(values);

        static IList<T> SelectCandidates(IEnumerable<T> values)
        {
            var candidates = values.ToList();

            if (candidates.Any(value => value is IHasRarity { Rarity: Rarity.Force }))
            {
                return candidates
                    .Where(value => value is IHasRarity { Rarity: Rarity.Force })
                    .ToList();
            }

            return candidates;
        }

        public T Pick(int? seed = null)
        {
            var random = seed == null ? new Random() : new Random((int)seed);

            return _values[random.Next(_values.Count)];
        }
    }
}

using static System.Math;
using System.Collections.Generic;
using System.Linq;

namespace Dose.Library.DataStructures
{
    public static class Utils
    {
        public static IEnumerable<IEnumerable<T>> SplitEnumerable<T>(IEnumerable<T> source, int n)
        {
            int chunkSize = (int)Ceiling((double)source.Count() / n);

            return Enumerable.Range(0, n)
                .Select(i => source.Skip(i * chunkSize))
                .Take(chunkSize)
                .Where(c => c.Any());
        }
    }
}
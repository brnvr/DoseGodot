using System;
using Dose.Library.Numbers;

namespace Dose.Library.Randomness
{
    public class RandomDecider(Probability probability, int? seed = null)
    {
        public Probability Probability { get; } = probability;
        readonly Random _rng = seed.HasValue ? new Random(seed.Value) : new Random();

        public bool Roll()
        {
            return _rng.NextDouble() < Probability;
        }
    }
}
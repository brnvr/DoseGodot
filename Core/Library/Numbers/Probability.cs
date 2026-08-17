using System;

namespace Dose.Core.Library.Numbers
{
    public readonly struct Probability
    {
        private readonly double _value;

        public double Value => _value;

        public Probability(double value)
        {
            if (double.IsNaN(value) || value is < 0.0 or > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Probability must be between 0 and 1.");
            }
                
            _value = value;
        }

        public static implicit operator Probability(double value) => new(value);

        public static implicit operator double(Probability probability) => probability._value;

        public override string ToString() => $"{_value:P}";
        public override int GetHashCode() => _value.GetHashCode();
        public override bool Equals(object? obj)
        {
            return obj is Probability p && _value.Equals(p._value);
        }

        public static bool operator ==(Probability a, Probability b) => a._value == b._value;
        public static bool operator !=(Probability a, Probability b) => a._value != b._value;
    }
}

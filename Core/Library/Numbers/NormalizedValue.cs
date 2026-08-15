using System;

namespace Dose.Core.Library.Numbers
{
    public readonly struct NormalizedValue : IEquatable<NormalizedValue>
    {
        readonly float _value;

        public float Value => _value;

        public NormalizedValue(float value)
        {
            if (float.IsNaN(value) || value < 0f || value > 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "Normalized value must be between 0 and 1."
                );
            }

            _value = value;
        }

        public static implicit operator NormalizedValue(float value) => new(value);
        public static implicit operator float(NormalizedValue value) => value._value;

        public bool Equals(NormalizedValue other) => _value.Equals(other._value);
        public override bool Equals(object? obj) => obj is NormalizedValue other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value.ToString();

        public static bool operator ==(NormalizedValue left, NormalizedValue right) => left.Equals(right);
        public static bool operator !=(NormalizedValue left, NormalizedValue right) => !left.Equals(right);
    }
}

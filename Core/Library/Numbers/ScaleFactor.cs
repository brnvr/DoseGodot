using System;

namespace Dose.Core.Library.Numbers
{
    public readonly struct ScaleFactor : IEquatable<ScaleFactor>
    {
        readonly float _value;

        public float Value => _value;

        public ScaleFactor(float value)
        {
            if (!float.IsFinite(value) || value <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "Scale factor must be finite and greater than zero."
                );
            }

            _value = value;
        }

        public static implicit operator ScaleFactor(float value) => new(value);
        public static implicit operator float(ScaleFactor scale) => scale._value;

        public bool Equals(ScaleFactor other) => _value.Equals(other._value);
        public override bool Equals(object? obj) => obj is ScaleFactor other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value.ToString();

        public static bool operator ==(ScaleFactor left, ScaleFactor right) => left.Equals(right);
        public static bool operator !=(ScaleFactor left, ScaleFactor right) => !left.Equals(right);
    }
}

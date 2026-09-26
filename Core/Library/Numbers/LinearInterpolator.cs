using System;
using Godot;

namespace Dose.Core.Library.Numbers
{
    public readonly struct LinearInterpolator : IInterpolator<float>
    {
        public float Value { get; }
        public float Target { get; }
        public float Delta { get; }

        public LinearInterpolator(float value, float target, float delta)
        {
            if (delta < 0)
            {
                throw new Exception($"{nameof(delta)} must be greater than 0.");
            }

            Value = value;
            Target = target;
            Delta = delta;
        }

        public float Interpolate() => Mathf.MoveToward(Value, Target, Delta);
    }
}

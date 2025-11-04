using System;

namespace Dose.Core.Library.Numbers
{
    public class LinearInterpolator : IInterpolator<double>
    {
        public double Value { get; }
        public double Target { get; }
        public double Delta { get; }

        public LinearInterpolator(double value, double target, double delta)
        {
            if (delta < 0)
            {
                throw new Exception($"{nameof(delta)} must be greater than 0.");
            }

            Value = value;
            Target = target;
            Delta = delta;
        }

        public double Interpolate()
        {
            return Math.Clamp(
                Value + Math.Sign(Target - Value) * Delta,
                Math.Min(Value, Target),
                Math.Max(Value, Target)
            );
        }
    }
}
using System.Numerics;

namespace Dose.Library.Numbers
{
    public interface IInterpolator<T> where T : INumber<T>
    {
        T Interpolate();
    }
}
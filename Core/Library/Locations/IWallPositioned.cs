using Dose.Core.Library.Numbers;

namespace Dose.Core.Library.Locations
{
    public interface IWallPositioned
    {
        NormalizedValue MinDistanceFromWall { get; }
        NormalizedValue MaxDistanceFromWall { get; }
        NormalizedValue MinDistanceFromCorner { get; }
        NormalizedValue MaxDistanceFromCorner { get; }
    }
}

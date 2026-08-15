using Dose.Core.Library.Numbers;

namespace Dose.Core.Library.Locations
{
    public interface IWallPositioned
    {
        NormalizedValue MinDistanceToWall { get; }
        NormalizedValue MaxDistanceToWall { get; }
        NormalizedValue MinDistanceFromCorner { get; }
        NormalizedValue MaxDistanceFromCorner { get; }
    }
}

using Dose.Core.Library.Actors;
using Dose.Core.Library.Location;

namespace Dose.Core.Library.Locations
{
    public interface IStairs : IPortal
    {
        StairsContext? Context { get; }
    }
}
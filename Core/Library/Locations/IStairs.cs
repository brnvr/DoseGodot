using Dose.Core.Library.Actors;

namespace Dose.Core.Library.Locations
{
    public interface IStairs : IPortal
    {
        StairsContext? Context { get; }
        public StairsDirection Direction { get; }
    }
}
using Dose.Core.Library.Locations;

namespace Dose.Core.Library.Location
{
    public interface IPortal
    {
        IPortal? Linked { get; }
        void Link(IPortal other);
        void Unlink();
        ILocation? Location { get; }
    }
}
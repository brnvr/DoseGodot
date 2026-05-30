namespace Dose.Core.Library.Locations
{
    public interface IPortal
    {
        IPortal? Linked { get; }
        void Link(IPortal other);
        void Unlink();
        ILocation? Location { get; }
    }
}
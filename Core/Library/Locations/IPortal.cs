namespace Dose.Core.Library.Locations
{
    public interface IPortal
    {
        Godot.Vector2I MapPosition { get; set; }
        GeoPositioning.CardinalDirection MapDirection { get; set; }
        IPortal? Linked { get; }
        void Link(IPortal other);
        void Unlink();
        ILocation? Location { get; }
    }
}

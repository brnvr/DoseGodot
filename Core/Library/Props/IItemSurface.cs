using Godot;

namespace Dose.Core.Library.Props
{
    public interface IItemSurface
    {
        bool CanPlaceItems { get; }
        bool TryGetSurfacePosition(Node3D relativeTo, out Vector3 position);
    }
}

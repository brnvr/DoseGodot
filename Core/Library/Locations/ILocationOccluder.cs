using Godot;

namespace Dose.Core.Library.Locations
{
    public interface ILocationOccluder
    {
        ILocationOccluder Initialize(Mesh wallMesh, Mesh doorwayMesh, Mesh floorMesh, Mesh ceilingMesh, int blockSize, StandardMaterial3D material);
    }
}
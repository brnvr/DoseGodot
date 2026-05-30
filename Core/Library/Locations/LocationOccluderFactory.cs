using Godot;

namespace Dose.Core.Library.Locations
{
    public class LocationOccluderFactory<T>(
        PackedScene scene,
        Mesh wallMesh,
        Mesh doorwayMesh,
        Mesh floorMesh,
        Mesh ceilingMesh,
        StandardMaterial3D material
    ) where T : Node3D, ILocationOccluder
    {
        public PackedScene Scene { get; } = scene;
        public Mesh WallMesh { get; } = wallMesh;
        public Mesh DoorwayMesh { get; } = doorwayMesh;
        public Mesh FloorMesh { get; } = floorMesh;
        public Mesh CeilingMesh { get; } = ceilingMesh;
        public StandardMaterial3D Material { get; } = material;

        public T? Build(int blockSize)
        {
            var occluderNode = Scene.Instantiate<T>();

            if (occluderNode is not ILocationOccluder occluder)
            {
                GD.PushWarning($"{nameof(Scene)} doesn't implement {nameof(ILocationOccluder)}. Skipping occluder building...");
                return null;
            }

            return (T)occluder.Initialize(WallMesh, DoorwayMesh, FloorMesh, CeilingMesh, blockSize, Material);
        }
    }
}
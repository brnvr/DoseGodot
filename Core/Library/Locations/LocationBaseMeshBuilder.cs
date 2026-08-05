using Godot;

namespace Dose.Core.Library.Locations
{
    public class LocationBaseMeshBuilder(
        IBuildableLocation location,
        Mesh mesh,
        StandardMaterial3D material,
        int blockSize,
        bool isCollidable,
        string? meshInstanceName = null
    ) : LocationMeshBuilder(location, material, isCollidable, meshInstanceName)
    {
        public int BlockSize { get; } = blockSize;
        public Mesh Mesh { get; } = mesh;


        protected override void OnBuildCell(Vector2I point)
        {
            if (Location.Map == null)
            {
                GD.PushWarning($"{nameof(Location.Map)} not defined. Skipping {nameof(OnBuildCell)}.");
                return;
            }
    
            var position = new Vector3((point.X - Location.Map.CenterX) * BlockSize, 0, (point.Y - Location.Map.CenterY) * BlockSize);
    
            var scale = new Vector3(BlockSize, BlockSize, BlockSize);
            var transform = new Transform3D(Basis.Identity.Scaled(scale), position);

            _surfaceTool.AppendFrom(Mesh, 0, transform);
        }
    }
}
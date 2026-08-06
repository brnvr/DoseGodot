using System.Linq;
using Godot;
namespace Dose.Core.Library.Locations
{
    public static class LocationCollisionLayer
    {
        public const uint Physics = 1u << 0;
        public const uint NavigationSource = 1u << 1;
    }

    public abstract class LocationMeshBuilder(
        IBuildableLocation location,
        StandardMaterial3D material,
        bool isCollidable,
        string? meshInstanceName,
        uint collisionLayer = LocationCollisionLayer.Physics
    ) : LocationBuilder(location)
    {
        protected SurfaceTool _surfaceTool = new();

        public bool IsCollidable { get; } = isCollidable;
        public uint CollisionLayer { get; } = collisionLayer;
        public string? MeshInstanceName { get; } = meshInstanceName;
        public StandardMaterial3D Material { get; } = material;

        protected override void OnStart() {
            _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
            _surfaceTool.SetSmoothGroup(uint.MaxValue);
        }

        protected override void OnFinish()
        {
            var meshInstance = new MeshInstance3D();

            if (MeshInstanceName != null)
            {
                meshInstance.Name = MeshInstanceName;
            }

            var mesh = _surfaceTool.Commit();

            mesh.SurfaceSetMaterial(0, Material);

            meshInstance.Mesh = mesh;
            
            if (IsCollidable)
            {
                meshInstance.CreateTrimeshCollision();

                foreach (var body in meshInstance.GetChildren().OfType<StaticBody3D>())
                {
                    body.CollisionLayer = CollisionLayer;
                }
            }

            Location.OnBuild(meshInstance);
        }
    }
}

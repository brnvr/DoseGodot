using Godot;
using Dose.Core.Library.Common;
using Dose.Core.Library.Props;
using Dose.Core.Library.Locations;
using Dose.Core.Library.Numbers;
using System.Linq;
using static Dose.Core.Library.GodotUtils;

namespace Dose.Core.Scenes
{
    public partial class Prop : RigidBody3D, IWallPositioned, IItemSurface, IInitializable<PropDefinition>
    {
        public PropDefinition? Definition { get; private set; }
        public bool CanPlaceItems => Definition?.SurfaceOffset != null;
        public NormalizedValue MinDistanceFromWall => Definition?.MinDistanceFromWall ?? 0f;
        public NormalizedValue MaxDistanceFromWall => Definition?.MaxDistanceFromWall ?? 1f;
        public NormalizedValue MinDistanceFromCorner => Definition?.MinDistanceFromCorner ?? 0f;
        public NormalizedValue MaxDistanceFromCorner => Definition?.MaxDistanceFromCorner ?? 1f;

        public bool TryGetSurfacePosition(Node3D relativeTo, out Vector3 position)
        {
            position = default;

            if (Definition?.SurfaceOffset is not Vector2 relativeOffset)
            {
                return false;
            }

            var minimum = new Vector3(
                float.PositiveInfinity,
                float.PositiveInfinity,
                float.PositiveInfinity
            );
            var maximum = new Vector3(
                float.NegativeInfinity,
                float.NegativeInfinity,
                float.NegativeInfinity
            );
            var hasBounds = false;

            foreach (var mesh in FindChildren("*", "MeshInstance3D", true, false)
                .OfType<MeshInstance3D>()
                .Where(mesh => mesh.Visible && mesh.Mesh != null))
            {
                if (!TryGetTransformRelativeTo(mesh, this, out var meshTransform))
                {
                    continue;
                }

                var bounds = mesh.GetAabb();

                for (var endpointIndex = 0; endpointIndex < 8; endpointIndex++)
                {
                    var endpoint = meshTransform * bounds.GetEndpoint(endpointIndex);
                    minimum = minimum.Min(endpoint);
                    maximum = maximum.Max(endpoint);
                    hasBounds = true;
                }
            }

            if (!hasBounds)
            {
                GD.PushWarning("A prop with SurfaceOffset has no visible mesh bounds.");
                return false;
            }

            var center = (minimum + maximum) * 0.5f;
            var localSurfacePosition = new Vector3(
                center.X + (maximum.X - minimum.X) * relativeOffset.X,
                center.Y + (maximum.Y - minimum.Y) * relativeOffset.Y,
                center.Z
            );

            if (!TryGetTransformRelativeTo(this, relativeTo, out var propTransform))
            {
                return false;
            }

            position = propTransform * localSurfacePosition;

            return true;
        }

        static bool TryGetTransformRelativeTo(
            Node3D node,
            Node3D ancestor,
            out Transform3D transform
        )
        {
            transform = node.Transform;
            var parent = node.GetParent();

            while (parent != ancestor)
            {
                if (parent == null)
                {
                    return false;
                }

                if (parent is Node3D parent3D)
                {
                    transform = parent3D.Transform * transform;
                }

                parent = parent.GetParent();
            }

            return true;
        }

        public Prop Initialize(PropDefinition definition)
        {
            Definition = definition;

            var billboardMesh = GetNode<MeshInstance3D>("Mesh");
            var billboardCollision = GetNode<CollisionShape3D>("CollisionShape3D");
            var scale = Vector3.One * definition.Scale;

            if (definition.IsBillboard)
            {
                billboardMesh.Scale *= scale;
                billboardMesh.Position *= definition.Scale;
                billboardMesh.Position += Vector3.Up * definition.YOffset;
                billboardCollision.Scale *= scale;
                billboardCollision.Position *= definition.Scale;
                billboardCollision.Position += Vector3.Up * definition.YOffset;
                billboardMesh.SetSurfaceOverrideMaterial(0, CreateBillboardMaterial(definition.Texture));
                return this;
            }

            billboardMesh.Visible = false;
            billboardCollision.Disabled = true;

            var modelScene = definition.ModelScene
                ?? throw new System.InvalidOperationException("A model prop has no loaded model scene.");

            var model = modelScene.Instantiate<Node3D>();
            model.Scale *= scale;
            model.Position += Vector3.Up * definition.YOffset;
            AddChild(model);

            return this;
        }

        void IInitializable<PropDefinition>.Initialize(PropDefinition definition) => Initialize(definition);
    }
}
